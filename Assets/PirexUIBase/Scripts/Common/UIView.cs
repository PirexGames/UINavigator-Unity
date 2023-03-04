using Cysharp.Threading.Tasks;
using UnityEngine;

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
        public virtual async UniTask OnShow()
        {
            if (animationTransition.ShowAnimation)
                await animationTransition.ShowAnimation.Excute(viewCG, ScreenContainer.Container.RectContainer);
            viewCG.alpha = 1;
            await UniTask.Yield();
        }

        public virtual async UniTask OnHide()
        {
            if (animationTransition.HideAnimation)
                await animationTransition.HideAnimation.Excute(viewCG, ScreenContainer.Container.RectContainer);
            viewCG.alpha = 0;
            await UniTask.Yield();
        }
    }
}