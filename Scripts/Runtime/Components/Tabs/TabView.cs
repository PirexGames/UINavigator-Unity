using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class TabView : UIView
    {
        public string TabData { get; set; }

        [Header("Tab Animation")]
        [SerializeField] protected AnimationContainerEView tabAnimation;

        public virtual async UniTask OnShow()
        {
            await WaitInitialize();
            if (this == null) return;

            SetActive(true);
            
            if (tabAnimation != null && tabAnimation.ShowAnimation != null)
                await tabAnimation.ShowAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            else
            {
                View.alpha = 1f;
            }
            View.blocksRaycasts = true;
        }

        public virtual async UniTask OnHide()
        {
            if (this == null) return;

            if (tabAnimation != null && tabAnimation.HideAnimation != null)
                await tabAnimation.HideAnimation.Excute(View, Owner != null ? Owner.RectContainer : null);
            
            SetActive(false);
        }
    }
}