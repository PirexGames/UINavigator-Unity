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

        public virtual async UniTask OnPushEnter()
        {
            await WaitInitialize();
            if (this == null) return;
            SetActive(true);

            if (modalAnimation != null && modalAnimation.PushEnterAnimation != null)
                await modalAnimation.PushEnterAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            else
            {
                View.alpha = 1;
                View.blocksRaycasts = true;
                View.interactable = true;
            }
        }

        public virtual async UniTask OnPushExit(ModalBaseView screenBelow)
        {
            if (this == null) return;

            if (modalAnimation != null && modalAnimation.PushExitAnimation != null)
                await modalAnimation.PushExitAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            else
            {
                View.alpha = 0;
                View.interactable = false;
                View.blocksRaycasts = false;
            }
        }

        public virtual async UniTask OnPopEnter()
        {
            await WaitInitialize();
            if (this == null) return;
            SetActive(true);

            if (modalAnimation != null && modalAnimation.PopEnterAnimation != null)
                await modalAnimation.PopEnterAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            else
            {
                View.alpha = 1;
                View.blocksRaycasts = true;
                View.interactable = true;
            }
        }

        public virtual async UniTask OnPopExit()
        {
            if (this == null) return;

            if (modalAnimation != null && modalAnimation.PopExitAnimation != null)
                await modalAnimation.PopExitAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            
            SetActive(false);
        }
    }
}
