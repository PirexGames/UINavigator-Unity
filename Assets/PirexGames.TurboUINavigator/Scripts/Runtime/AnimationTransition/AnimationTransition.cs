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

        public float Duration => duration;

        public virtual async UniTask Excute(CanvasGroup target, RectTransform container = null)
        {
            target.alpha = alphaBefore;
            target.transform.localScale = scaleBefore;
            target.transform.localPosition = GetPosition(positionBefore, (RectTransform)target.transform, container, offsetBefore);

            target.DOFade(alphaAfter, duration);
            target.transform.DOScale(scaleAfter, duration);

            if (container == null)
            {
                container = (RectTransform)target.transform;
            }

            await target.transform.DOLocalMove
                (GetPosition(positionAfter, (RectTransform)target.transform, container, offsetAfter), duration)
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

            target.DOFade(alphaAfter, duration);
            await target.transform.DOScale(scaleAfter, duration).AsyncWaitForCompletion();
        }

        private Vector3 GetPosition(PositionTransitionEnum position, RectTransform target, RectTransform container, Vector3 offset)
        {
            float x = 0;
            float y = 0;
            float z = 0;
            switch (position)
            {
                case PositionTransitionEnum.CENTER:
                    break;
                case PositionTransitionEnum.TOP:
                    y = target.rect.height / 2 + container.rect.height / 2;
                    break;
                case PositionTransitionEnum.BOTTOM:
                    y = -target.rect.height / 2 - container.rect.height / 2;
                    break;
                case PositionTransitionEnum.LEFT:
                    x = -target.rect.width / 2 - container.rect.width / 2;
                    break;
                case PositionTransitionEnum.RIGHT:
                    x = target.rect.width / 2 + container.rect.width / 2;
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