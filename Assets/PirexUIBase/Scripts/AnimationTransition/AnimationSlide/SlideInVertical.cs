using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "SlideInVerticalAnimationSO", menuName = "Pirex.UI.Base/Animation/SlideInVerticalAnimationSO")]
public class SlideInVertical : AnimationTransition
{
    [SerializeField] private SlideInVerEnum typeSlide;
    public override async UniTask Excute(CanvasGroup target, RectTransform container = null)
    {
        if (target.transform is RectTransform rect)
        {
            var posStart = rect.rect.height / 2 + container.rect.height / 2;
            if (typeSlide == SlideInVerEnum.FROMBOTTOM) posStart *= -1;
            target.transform.DOLocalMoveY(posStart, 0);
        }
        await UniTask.NextFrame();
        target.alpha = 1;
        target.blocksRaycasts = true;
        await target.transform
            .DOLocalMoveY(0, duration)
            .AsyncWaitForCompletion();
    }

    public override UniTask Excute(CanvasGroup target, Vector2 position, bool isLocalPosition = false)
    {
        throw new System.NotImplementedException();
    }

    enum SlideInVerEnum
    {
        FROMTOP,
        FROMBOTTOM,
    }
}