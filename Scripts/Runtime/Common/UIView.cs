using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Pirex.UI.Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIView : MonoBehaviour
    {

        [Header("Animation Transition")]
        [SerializeField] protected AnimationContainer animationTransition;
        protected CanvasGroup viewCG;
        protected virtual void Awake()
        {
            viewCG = this.GetComponent<CanvasGroup>();
            viewCG.alpha = 0;
        }

        protected virtual void OnEnable()
        {
            viewCG.alpha = 0;
        }

        public virtual async UniTask OnPushEnter()
        {
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
            if (animationTransition.PushEnterAnimation)
                await animationTransition.PushEnterAnimation.Excute(viewCG, position, isLocalPosition);
            else
            {
                transform.localScale = Vector3.one;
                viewCG.alpha = 1;
            }

            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnPushExit()
        {
            if (animationTransition.PopExitAnimation)
                animationTransition.PushExitAnimation.Excute(viewCG, ScreenContainer.Container.RectContainer).Forget();
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnPopEnter()
        {
            if (animationTransition.PushEnterAnimation)
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
            if (animationTransition.PushEnterAnimation)
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
            if (animationTransition.PopExitAnimation)
                await animationTransition.PopExitAnimation.Excute(viewCG, ScreenContainer.Container.RectContainer);
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnPopExit(Vector2 position, bool isLocalPosition = false)
        {
            if (animationTransition.PushEnterAnimation)
                await animationTransition.PopExitAnimation.Excute(viewCG, position, isLocalPosition);
            else
            {
                transform.localScale = Vector3.zero;
                viewCG.alpha = 1;
            }
            await UniTask.CompletedTask;
        }
    }
}