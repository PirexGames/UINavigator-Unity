using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIView : MonoBehaviour
    {
        [SerializeField] protected bool isPooling = false;
        
        public UIContainer Owner { get; set; }
        public string NameAddressable { get; set; }
        public bool IsPooling => isPooling;
        
        protected CanvasGroup View;
        protected bool IsInitialized;

        private System.Threading.CancellationTokenSource _sessionCts;
        private System.Threading.CancellationTokenSource _transitionCts;

        /// <summary>
        /// Global token for the life of the GameObject
        /// </summary>
        private System.Threading.CancellationToken DestroyToken => this.GetCancellationTokenOnDestroy();

        /// <summary>
        /// Token for the current active session (reset on each DoInitialize)
        /// </summary>
        internal System.Threading.CancellationToken SessionToken 
        {
            get
            {
                if (_sessionCts == null) _sessionCts = new System.Threading.CancellationTokenSource();
                return _sessionCts.Token;
            }
        }

        protected virtual void Awake()
        {
            View = this.GetComponent<CanvasGroup>();
            View.alpha = 0;
            IsInitialized = false;
        }

        protected virtual void OnDestroy()
        {
            CancelSession();
            if (_transitionCts != null)
            {
                _transitionCts.Cancel();
                _transitionCts.Dispose();
                _transitionCts = null;
            }
        }

        private void CancelSession()
        {
            if (_sessionCts != null)
            {
                _sessionCts.Cancel();
                _sessionCts.Dispose();
                _sessionCts = null;
            }
        }

        protected System.Threading.CancellationToken GetTransitionToken()
        {
            if (_transitionCts != null)
            {
                _transitionCts.Cancel();
                _transitionCts.Dispose();
            }
            _transitionCts = new System.Threading.CancellationTokenSource();
            return System.Threading.CancellationTokenSource.CreateLinkedTokenSource(_transitionCts.Token, DestroyToken).Token;
        }

        protected async UniTask WaitInitialize()
        {
            if (this == null || gameObject == null) return;
            // Wait until initialized OR the object is destroyed
            await UniTask.WaitUntil(() => IsInitialized || this == null || gameObject == null, cancellationToken: DestroyToken);
        }

        internal async UniTask DoInitialize()
        {
            if (this == null || gameObject == null) return;
            
            CancelSession(); // Cancel any lingering tasks from previous use
            await Initialize();
            
            if (this != null && gameObject != null)
            {
                IsInitialized = true;
            }
        }

        protected virtual async UniTask Initialize()
        {
            await UniTask.CompletedTask;
        }

        internal virtual void SetActive(bool active)
        {
            if (this == null || gameObject == null) return;
            gameObject.SetActive(active);
        }

        /// <summary>
        /// Adjusts the anchoredPosition to keep the view visually centered at its anchor point,
        /// compensating for the pivot offset without changing the anchors themselves.
        /// </summary>
        internal void ApplyPivotCenter()
        {
            var rt = transform as RectTransform;
            if (rt == null) return;

            // Calculate the pivot offset relative to the center (0.5).
            // This allows us to set anchoredPosition to the correct 'zero' for this pivot.
            Vector2 pivotOffset = new Vector2(rt.pivot.x - 0.5f, rt.pivot.y - 0.5f);
            rt.anchoredPosition = new Vector2(pivotOffset.x * rt.rect.width, pivotOffset.y * rt.rect.height);
        }

        /// <summary>
        /// Closes this view via its owner container.
        /// </summary>
        public async UniTask Close(bool awaitAnimation = true)
        {
            if (Owner != null)
            {
                await Owner.Close(this, awaitAnimation);
            }
            else
            {
                // Fallback if no owner (rare, but for safety)
                SetActive(false);
                if (IsPooling) UIContainer.ReturnToPool(NameAddressable, gameObject);
                else Destroy(gameObject);
            }
        }
    }
}