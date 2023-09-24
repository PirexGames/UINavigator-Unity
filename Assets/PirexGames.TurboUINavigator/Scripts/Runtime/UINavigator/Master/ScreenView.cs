using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class ScreenView : UIView
    {

        [Header("Animation Transition")]
        [SerializeField] protected AnimationContainerCView animationTransition;
        public virtual async UniTask OnPushEnter()
        {
            await UniTask.WaitUntil(() => isInitialized);
            if (animationTransition.PushEnterAnimation)
                await animationTransition.PushEnterAnimation.Excute(viewCG, ScreenContainer.Container.RectContainer);
            else
            {
                transform.localScale = Vector3.one;
                viewCG.alpha = 1;
            }
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnPushEnter(Vector2 position, bool isLocalPosition = false)
        {
            await UniTask.WaitUntil(() => isInitialized);
            this.gameObject.SetActive(true);
            if (animationTransition.PushEnterAnimation)
                await animationTransition.PushEnterAnimation.Excute(viewCG, position, isLocalPosition);
            else
            {
                transform.localScale = Vector3.one;
                viewCG.alpha = 1;
            }
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnPushExit(Screen screen)
        {
            await UniTask.WaitUntil(() => screen.IsInitalized);
            if (animationTransition.PushExitAnimation)
                animationTransition.PushExitAnimation.Excute(viewCG, ScreenContainer.Container.RectContainer).Forget();
            await UniTask.CompletedTask;
            this.gameObject.SetActive(false);
        }

        public virtual async UniTask OnPopEnter()
        {
            await UniTask.WaitUntil(() => isInitialized);
            this.gameObject.SetActive(true);
            if (animationTransition.PopEnterAnimation)
                animationTransition.PopEnterAnimation.Excute(viewCG, ScreenContainer.Container.RectContainer).Forget();
            else
            {
                transform.localScale = Vector3.one;
                viewCG.alpha = 1;
            }
            await UniTask.CompletedTask;
        }
        public virtual async UniTask OnPopEnter(Vector2 position, bool isLocalPosition = false)
        {
            await UniTask.WaitUntil(() => isInitialized);
            this.gameObject.SetActive(true);
            if (animationTransition.PopEnterAnimation)
                await animationTransition.PopEnterAnimation.Excute(viewCG, position, isLocalPosition);
            else
            {
                transform.localScale = Vector3.one;
                viewCG.alpha = 1;
            }
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnPopExit()
        {
            await UniTask.WaitUntil(() => isInitialized);
            if (animationTransition.PopExitAnimation)
                await animationTransition.PopExitAnimation.Excute(viewCG, ScreenContainer.Container.RectContainer);
            this.gameObject.SetActive(false);
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnPopExit(Vector2 position, bool isLocalPosition = false)
        {
            await UniTask.WaitUntil(() => isInitialized);
            if (animationTransition.PopExitAnimation)
                await animationTransition.PopExitAnimation.Excute(viewCG, position, isLocalPosition);
            else
            {
                transform.localScale = Vector3.zero;
                viewCG.alpha = 1;
            }
            this.gameObject.SetActive(false);
            await UniTask.CompletedTask;
        }
    }
}
