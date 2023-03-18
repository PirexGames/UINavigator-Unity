using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "AnimationTransition", menuName = "Pirex.UI.Base/Animation/AnimationTransition")]
public class AnimationTransition : ScriptableObject
{
    [SerializeField] protected float duration = .15f;

    [Header("Before Properties")]
    [SerializeField] protected float alphaBefore = 0f;
    [SerializeField] protected Vector3 scaleBefore;
    [SerializeField] protected PositionTransitionEnum positionBefore;

    [Header("After Properties")]
    [SerializeField] protected float alphaAfter = 1f;
    [SerializeField] protected Vector3 scaleAfter;
    [SerializeField] protected PositionTransitionEnum positionAfter;

    public virtual async UniTask Excute(CanvasGroup target, RectTransform container = null)
    {
        target.alpha = alphaBefore;
        target.transform.localScale = scaleBefore;
        target.transform.localPosition = GetPosition(positionBefore, (RectTransform)target.transform, container);

        target.DOFade(alphaAfter, duration);
        target.transform.DOScale(scaleAfter, duration);

        if (container == null)
        {
            container = (RectTransform)target.transform.parent;
        }

        await target.transform.DOLocalMove
            (GetPosition(positionAfter, (RectTransform)target.transform, container), duration)
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

    private Vector3 GetPosition(PositionTransitionEnum position, RectTransform target, RectTransform container)
    {
        float x = 0;
        float y = 0;
        switch (position)
        {
            case PositionTransitionEnum.CENTER:
                return Vector3.zero;
            case PositionTransitionEnum.TOP:
                y = target.rect.height / 2 + container.rect.height / 2;
                return new Vector3(0, y, 0);
            case PositionTransitionEnum.BOTTOM:
                y = -target.rect.height / 2 - container.rect.height / 2;
                return new Vector3(0, y, 0);
            case PositionTransitionEnum.LEFT:
                x = -target.rect.width / 2 - container.rect.width / 2;
                return new Vector3(x, 0, 0);
            case PositionTransitionEnum.RIGHT:
                x = target.rect.width / 2 + container.rect.width / 2;
                return new Vector3(x, 0, 0);
            default:
                return Vector3.zero;
        }
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