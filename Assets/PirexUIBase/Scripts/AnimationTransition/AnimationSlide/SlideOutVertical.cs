using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "SlideOutVerticalAnimationSO", menuName = "Pirex.UI.Base/Animation/SlideOutVerticalAnimationSO")]
public class SlideOutVertical : AnimationTransition
{
    [SerializeField] private SlideOutVerEnum typeSlide;
    public override async UniTask Excute(CanvasGroup target, RectTransform container = null)
    {
        target.blocksRaycasts = false;
        if (target.transform is RectTransform rect)
        {
            var posEnd = rect.rect.height / 2 + container.rect.height / 2;
            if (typeSlide == SlideOutVerEnum.TOBOTTOM) posEnd *= -1;
            await target.transform
                .DOLocalMoveY(posEnd, durationOut)
                .AsyncWaitForCompletion();
        }
        else
        {
            await target.transform
                .DOLocalMoveY(container.rect.width, durationOut)
                .AsyncWaitForCompletion();
        }

    }
    enum SlideOutVerEnum
    {
        TOTOP,
        TOBOTTOM,
    }
}