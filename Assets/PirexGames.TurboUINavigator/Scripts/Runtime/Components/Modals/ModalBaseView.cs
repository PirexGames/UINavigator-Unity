using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class ModalBaseView : UIView
    {
        [Header("Modal Animation (Simplified)")]
        [SerializeField] protected AnimationContainerEView modalAnimation;
        public CanvasGroup Backdrop { get; internal set; }
        public bool CloseOnClickBackdrop { get; set; } = false;

        public virtual async UniTask OnPushEnter()
        {
            await WaitInitialize();
            if (this == null) return;
            SetActive(true);
            if (modalAnimation != null && modalAnimation.ShowAnimation != null)
                await modalAnimation.ShowAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            
            if (this == null) return;
            if (modalAnimation == null || modalAnimation.ShowAnimation == null)
            {
                transform.localScale = Vector3.one;
                View.alpha = 1;
                View.blocksRaycasts = true;
                View.interactable = true;
            }
        }

        public virtual async UniTask OnPushEnter(Vector2 position, bool isLocalPosition = false)
        {
            await WaitInitialize();
            SetActive(true);
            if (modalAnimation != null && modalAnimation.ShowAnimation != null)
                await modalAnimation.ShowAnimation.Excute(View, position, isLocalPosition);
            else
            {
                transform.localScale = Vector3.one;
                View.alpha = 1;
            }
        }

        public virtual async UniTask OnPushExit(ModalBaseView screenBelow)
        {
            if (this == null) return;
            // Modals usually don't animate when something is pushed on top of them,
            // but we must disable it to save performance.
            SetActive(false);
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnPopEnter()
        {
            await WaitInitialize();
            SetActive(true);
            View.alpha = 1;
            View.blocksRaycasts = true;
            View.interactable = true;
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnPopEnter(Vector2 position, bool isLocalPosition = false)
        {
            await WaitInitialize();
            SetActive(true);
            View.alpha = 1;
        }

        public virtual async UniTask OnPopExit()
        {
            if (this == null) return;
            if (modalAnimation != null && modalAnimation.HideAnimation != null)
                await modalAnimation.HideAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            
            SetActive(false);
        }

        public virtual async UniTask OnPopExit(Vector2 position, bool isLocalPosition = false)
        {
            if (this == null) return;
            if (modalAnimation != null && modalAnimation.HideAnimation != null)
                await modalAnimation.HideAnimation.Excute(View, position, isLocalPosition);
            else
            {
                transform.localScale = Vector3.zero;
                View.alpha = 1;
            }
            SetActive(false);
        }
    }
}
