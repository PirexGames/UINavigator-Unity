using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pirex.UI.Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIView : MonoBehaviour
    {

        [Header("Animation Transition")]
        [SerializeField] protected AnimationTransition animationTransition;
        protected CanvasGroup viewCG;
        protected virtual void Awake()
        {
            viewCG = this.GetComponent<CanvasGroup>();
            viewCG.alpha = 0;
        }
        public virtual async UniTask OnShow()
        {
            if (animationTransition is not null)
                await animationTransition.On(viewCG, ScreenContainer.rectContainer);
            viewCG.alpha = 1;
            await UniTask.Yield();
        }

        public virtual async UniTask OnHide()
        {
            if (animationTransition is not null)
                await animationTransition.Off(viewCG, ScreenContainer.rectContainer);
            viewCG.alpha = 0;
            await UniTask.Yield();
        }
    }
}