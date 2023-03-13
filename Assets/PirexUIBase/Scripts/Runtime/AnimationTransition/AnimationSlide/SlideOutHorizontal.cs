
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "SlideOutHorizontalAnimationSO", menuName = "Pirex.UI.Base/Animation/SlideOutHorizontalAnimationSO")]
public class SlideOutHorizontal : AnimationTransition
{
    [SerializeField] private SlideOutHorEnum typeSlide;
    public override async UniTask Excute(CanvasGroup target, RectTransform container = null)
    {
        target.blocksRaycasts = false;
        if (target.transform is RectTransform rect)
        {
            var posEnd = rect.rect.width / 2 + container.rect.width / 2;
            if (typeSlide == SlideOutHorEnum.TOLEFT) posEnd *= -1;
            await target.transform
                .DOLocalMoveX(posEnd, duration)
                .AsyncWaitForCompletion();
        }
        else
        {
            await target.transform
                .DOLocalMoveX(container.rect.width, duration)
                .AsyncWaitForCompletion();
        }

    }

    public override UniTask Excute(CanvasGroup target, Vector2 position, bool isLocalPosition = false)
    {
        throw new System.NotImplementedException();
    }

    enum SlideOutHorEnum
    {
        TOLEFT,
        TORIGHT,
    }
}