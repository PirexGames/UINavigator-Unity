using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pirex.UI.Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Tab : MonoBehaviour
    {
        [Header("Animation Transition")]
        [SerializeField] protected AnimationTab animationTransition;
        public Toggle toggleThisTab;
        protected CanvasGroup viewCG;
        protected virtual void Awake()
        {
            viewCG = this.GetComponent<CanvasGroup>();
            viewCG.alpha = 0;
            viewCG.blocksRaycasts = false;
        }
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

        public virtual async UniTask OnHide()
        {
            if (animationTransition.HideAnimation)
                await animationTransition.HideAnimation.Excute(viewCG);
            await UniTask.CompletedTask;
        }
    }
}