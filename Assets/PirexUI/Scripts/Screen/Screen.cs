using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Pirex.UI.Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Screen : MonoBehaviour
    {
        [SerializeField] protected bool isHidenOnPushOther = true;
        [SerializeField] protected AnimationTransition transitionAnimation;
        protected CanvasGroup screenCG;
        protected virtual void Awake()
        {
            screenCG = this.GetComponent<CanvasGroup>();
            screenCG.alpha = 0;
        }
        public void OnPushOtherScreen()
        {
            gameObject.SetActive(!isHidenOnPushOther);
        }

        public virtual async UniTask OnPush()
        {
            if (transitionAnimation is not null)
                await transitionAnimation.On(screenCG, ScreenContainer.rectScreenContainer);
            screenCG.alpha = 1;
            await UniTask.Yield();
        }

        public virtual async UniTask OnPop()
        {
            if (transitionAnimation is not null)
                await transitionAnimation.Off(screenCG, ScreenContainer.rectScreenContainer);
            screenCG.alpha = 0;
            await UniTask.Yield();
        }
    }
}
