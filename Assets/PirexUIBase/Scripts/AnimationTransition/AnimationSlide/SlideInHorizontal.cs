using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "SlideInHorizontalAnimationSO", menuName = "Pirex.UI.Base/Animation/SlideInHorizontalAnimationSO")]
public class SlideInHorizontal : AnimationTransition
{
    [SerializeField] private SlideInHorEnum typeSlide;
    public override async UniTask Excute(CanvasGroup target, RectTransform container = null)
    {
        if (target.transform is RectTransform rect)
        {
            var posStart = rect.rect.width / 2 + container.rect.width / 2;
            if (typeSlide == SlideInHorEnum.FROMLEFT) posStart *= -1;
            target.transform.DOLocalMoveX(posStart, 0);
        }
        await UniTask.NextFrame();
        target.blocksRaycasts = true;
        target.alpha = 1;
        await target.transform
            .DOLocalMoveX(0, durationIn)
            .AsyncWaitForCompletion();
    }

    enum SlideInHorEnum
    {
        FROMLEFT,
        FROMRIGHT,
    }
}
