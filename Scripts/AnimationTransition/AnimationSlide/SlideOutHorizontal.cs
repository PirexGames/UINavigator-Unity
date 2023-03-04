
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
                .DOLocalMoveX(posEnd, durationOut)
                .AsyncWaitForCompletion();
        }
        else
        {
            await target.transform
                .DOLocalMoveX(container.rect.width, durationOut)
                .AsyncWaitForCompletion();
        }

    }
    enum SlideOutHorEnum
    {
        TOLEFT,
        TORIGHT,
    }
}