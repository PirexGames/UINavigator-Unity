using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class ScreenBaseView : UIView
    {
        [Header("Animation Transition")]
        [SerializeField] protected AnimationContainerCView animationTransition;

        public virtual async UniTask OnPushEnter()
        {
            await WaitInitialize();
            if (this == null) return;
            if (animationTransition.PushEnterAnimation)
                await animationTransition.PushEnterAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            
            if (this == null) return;
            if (!animationTransition.PushEnterAnimation)
            {
                transform.localScale = Vector3.one;
                View.alpha = 1;
            }
            View.blocksRaycasts = true;
        }

        public virtual async UniTask OnPushExit(ScreenBaseView screenBaseBelow)
        {
            if (this == null) return;
            View.blocksRaycasts = false; // Disable immediately to prevent user interaction during exit animation
            if (animationTransition.PushExitAnimation)
                await animationTransition.PushExitAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            
            SetActive(false);
        }

        public virtual async UniTask OnPopEnter()
        {
            await WaitInitialize();
            SetActive(true);
            if (animationTransition.PopEnterAnimation)
                await animationTransition.PopEnterAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            else
            {
                transform.localScale = Vector3.one;
                View.alpha = 1;
            }
            View.blocksRaycasts = true;
        }

        public virtual async UniTask OnPopExit()
        {
            if (this == null) return;
            if (animationTransition.PopExitAnimation)
                await animationTransition.PopExitAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            
            SetActive(false);
        }

    }
}
