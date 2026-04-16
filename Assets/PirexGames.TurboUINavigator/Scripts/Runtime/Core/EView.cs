using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class EView : UIView
    {
        [Header("Animation Transition")]
        [SerializeField] protected AnimationContainerEView animationTransition;

        internal virtual async UniTask OnShow(Vector2? position = null, bool isLocalPosition = false)
        {
            View.alpha = 0f;
            await WaitInitialize();
            
            if (View == null) return; // Safety check

            if (animationTransition.ShowAnimation)
            {
                if (position.HasValue)
                    await animationTransition.ShowAnimation.Excute(View, position.Value, isLocalPosition);
                else
                    await animationTransition.ShowAnimation.Excute(View);
            }
            else
            {
                transform.localScale = Vector3.one;
                View.alpha = 1;
            }
            View.interactable = true;
        }

        internal virtual async UniTask OnHide(Vector2? position = null, bool isLocalPosition = false)
        {
            await WaitInitialize();
            if (View == null) return; // Safety check

            if (animationTransition.HideAnimation)
            {
                if (position.HasValue)
                    await animationTransition.HideAnimation.Excute(View, position.Value, isLocalPosition);
                else
                    await animationTransition.HideAnimation.Excute(View);
            }
            else
            {
                View.alpha = 0;
            }
            View.interactable = false;
        }
    }
}
