using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "SlideVerticalAnimationSO", menuName = "Pirex.UI.Base/Animation/SlideVerticalAnimationSO")]
public class SlideVertical : AnimationTransition
{
    public override async UniTask Off(CanvasGroup target, RectTransform container = null)
    {
        target.blocksRaycasts = false;
        if (target.transform is RectTransform rect)
        {
            await target.transform
                .DOLocalMoveY(rect.rect.width / 2 + container.rect.width / 2, durationOut)
                .AsyncWaitForCompletion();
        }
        else
        {
            await target.transform
                .DOLocalMoveY(container.rect.width, durationOut)
                .AsyncWaitForCompletion();
        }

    }

    public override async UniTask On(CanvasGroup target, RectTransform container = null)
    {
        if (target.transform is RectTransform rect)
            target.transform.DOLocalMoveY(rect.rect.height / 2 + container.rect.height / 2, 0);
        await UniTask.NextFrame();
        target.alpha = 1;
        target.blocksRaycasts = true;
        await target.transform
            .DOLocalMoveY(0, durationIn)
            .AsyncWaitForCompletion();
    }
}