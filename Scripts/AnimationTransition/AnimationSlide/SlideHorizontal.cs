using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "SlideHorizontalAnimationSO", menuName = "Pirex.UI.Base/Animation/SlideHorizontalAnimationSO")]
public class SlideHorizontal : AnimationTransition
{
    public override async UniTask Off(CanvasGroup target, RectTransform container = null)
    {
        target.blocksRaycasts = false;
        if (target.transform is RectTransform rect)
        {
            await target.transform
                .DOLocalMoveX(rect.rect.width / 2 + container.rect.width / 2, durationOut)
                .AsyncWaitForCompletion();
        }
        else
        {
            await target.transform
                .DOLocalMoveX(container.rect.width, durationOut)
                .AsyncWaitForCompletion();
        }

    }

    public override async UniTask On(CanvasGroup target, RectTransform container = null)
    {
        if (target.transform is RectTransform rect)
            target.transform.DOLocalMoveX(rect.rect.width / 2 + container.rect.width / 2, 0);
        await UniTask.NextFrame();
        target.blocksRaycasts = true;
        target.alpha = 1;
        await target.transform
            .DOLocalMoveX(0, durationIn)
            .AsyncWaitForCompletion();
    }
}