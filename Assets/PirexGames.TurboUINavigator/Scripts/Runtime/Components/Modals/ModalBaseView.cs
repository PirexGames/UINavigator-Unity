using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class ModalBaseView : UIView
    {
        [Header("Modal Animation (CView)")]
        [SerializeField] protected AnimationContainerCView modalAnimation;
        public CanvasGroup Backdrop { get; internal set; }
        public bool CloseOnClickBackdrop { get; set; } = false;
        public bool EnableBackdrop { get; set; } = true;

        public virtual async UniTask OnPushEnter()
        {
            await WaitInitialize();
            if (this == null) return;
            SetActive(true);
            var token = GetTransitionToken();

            if (modalAnimation != null && modalAnimation.PushEnterAnimation != null)
                await modalAnimation.PushEnterAnimation.Excute(View, Owner != null ? Owner.RectContainer : null, token);
            else
            {
                View.alpha = 1;
            }
            
            if (this == null || token.IsCancellationRequested) return;
            View.blocksRaycasts = true;
        }

        public virtual async UniTask OnPushExit(ModalBaseView screenBelow)
        {
            if (this == null) return;
            View.blocksRaycasts = false;
            var token = GetTransitionToken();

            if (modalAnimation != null && modalAnimation.PushExitAnimation != null)
                await modalAnimation.PushExitAnimation.Excute(View, Owner != null ? Owner.RectContainer : null, token);
            else
            {
                View.alpha = 0;
            }
        }

        public virtual async UniTask OnPopEnter()
        {
            await WaitInitialize();
            if (this == null) return;
            SetActive(true);
            var token = GetTransitionToken();

            if (modalAnimation != null && modalAnimation.PopEnterAnimation != null)
                await modalAnimation.PopEnterAnimation.Excute(View, Owner != null ? Owner.RectContainer : null, token);
            else
            {
                View.alpha = 1;
            }
            
            if (this == null || token.IsCancellationRequested) return;
            View.blocksRaycasts = true;
        }

        public virtual async UniTask OnPopExit()
        {
            if (this == null) return;
            var token = GetTransitionToken();

            if (modalAnimation != null && modalAnimation.PopExitAnimation != null)
                await modalAnimation.PopExitAnimation.Excute(View, Owner != null ? Owner.RectContainer : null, token);
            
            if (this == null || token.IsCancellationRequested) return;
            SetActive(false);
        }
    }
}
