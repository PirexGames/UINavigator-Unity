using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pirex.UI.Base
{
    public class EView : UIView
    {
        [Header("Animation Transition")]
        [SerializeField] protected AnimationContainerEView animationTransition;
        public virtual async UniTask OnShow()
        {

            if (animationTransition.ShowAnimation)
                await animationTransition.ShowAnimation.Excute(viewCG);
            else
            {
                transform.localScale = Vector3.one;
                viewCG.alpha = 1;
            }
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnShow(Vector2 position, bool isLocalPosition = false)
        {
            if (animationTransition.ShowAnimation)
                await animationTransition.ShowAnimation.Excute(viewCG, position, isLocalPosition);
            else
            {
                transform.localScale = Vector3.one;
                viewCG.alpha = 1;
            }

            await UniTask.CompletedTask;
        }


        public virtual async UniTask OnHide()
        {
            if (animationTransition.HideAnimation)
                await animationTransition.HideAnimation.Excute(viewCG);
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnHide(Vector2 position, bool isLocalPosition = false)
        {
            if (animationTransition.HideAnimation)
                await animationTransition.HideAnimation.Excute(viewCG, position, isLocalPosition);
            else
            {
                transform.localScale = Vector3.zero;
                viewCG.alpha = 1;
            }
            await UniTask.CompletedTask;
        }
    }
}
