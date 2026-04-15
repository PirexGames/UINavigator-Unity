using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace PirexGames.Core.UINavigator
{

    [CreateAssetMenu(fileName = "AnimationTransition", menuName = "PirexGames.Core.UINavigator/Animation/AnimationTransition")]
    public class AnimationTransition : ScriptableObject
    {
        [SerializeField] protected float duration = .15f;

        [Header("Before Properties")]
        [SerializeField] protected float alphaBefore = 0f;
        [SerializeField] protected Vector3 scaleBefore;
        [SerializeField] protected PositionTransitionEnum positionBefore;
        [SerializeField] protected Vector3 offsetBefore;

        [Header("After Properties")]
        [SerializeField] protected float alphaAfter = 1f;
        [SerializeField] protected Vector3 scaleAfter;
        [SerializeField] protected PositionTransitionEnum positionAfter;
        [SerializeField] protected Vector3 offsetAfter;
        
        [Header("Ease")]
        [SerializeField] protected Ease ease;
        
        public float Duration => duration;

        public virtual async UniTask Excute(CanvasGroup target, RectTransform container = null)
        {
            if (container == null)
            {
                container = target.transform.parent as RectTransform;
                if (container == null) container = (RectTransform)target.transform;
            }

            target.alpha = alphaBefore;
            target.transform.localScale = scaleBefore;
            target.transform.localPosition = GetPosition(positionBefore, (RectTransform)target.transform, container, offsetBefore);

            target.DOFade(alphaAfter, duration).SetEase(ease);
            target.transform.DOScale(scaleAfter, duration).SetEase(ease);

            await target.transform.DOLocalMove
                (GetPosition(positionAfter, (RectTransform)target.transform, container, offsetAfter), duration)
                .SetEase(ease)
                .AsyncWaitForCompletion();
        }
        public async UniTask Excute(CanvasGroup target, Vector2 position, bool isLocalPosition = false)
        {
            target.alpha = alphaBefore;
            target.transform.localScale = scaleBefore;

            if (isLocalPosition)
                target.transform.localPosition = position;
            else
                target.transform.position = position;

            target.DOFade(alphaAfter, duration).SetEase(ease);
            await target.transform.DOScale(scaleAfter, duration).SetEase(ease).AsyncWaitForCompletion();
        }

        private Vector3 GetPosition(PositionTransitionEnum position, RectTransform target, RectTransform container, Vector3 offset)
        {
            float pivotX = target.pivot.x;
            float pivotY = target.pivot.y;
            float tWidth = target.rect.width;
            float tHeight = target.rect.height;
            float cWidth = container.rect.width;
            float cHeight = container.rect.height;

            // Start with balanced center coordinates
            float x = (pivotX - 0.5f) * tWidth;
            float y = (pivotY - 0.5f) * tHeight;
            float z = 0;

            switch (position)
            {
                case PositionTransitionEnum.CENTER:
                    // Already at balanced center
                    break;
                case PositionTransitionEnum.TOP:
                    y = (cHeight / 2) + (pivotY * tHeight);
                    break;
                case PositionTransitionEnum.BOTTOM:
                    y = (-cHeight / 2) - ((1 - pivotY) * tHeight);
                    break;
                case PositionTransitionEnum.LEFT:
                    x = (-cWidth / 2) - ((1 - pivotX) * tWidth);
                    break;
                case PositionTransitionEnum.RIGHT:
                    x = (cWidth / 2) + (pivotX * tWidth);
                    break;
                default:
                    break;
            }

            return new Vector3(x, y, z) + offset;
        }

        protected enum PositionTransitionEnum
        {
            CENTER,
            TOP,
            BOTTOM,
            LEFT,
            RIGHT,
        }
    }
}