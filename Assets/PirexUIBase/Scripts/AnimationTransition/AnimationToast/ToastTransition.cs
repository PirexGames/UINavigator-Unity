using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "ToastTransition", menuName = "Pirex.UI.Base/Animation/ToastTransitionSO")]
public class ToastTransition : AnimationTransition
{
    public async override UniTask On(CanvasGroup target, RectTransform container = null)
    {
        var rect = (RectTransform) target.transform;
        rect.localScale = Vector3.zero;
        target.alpha = 0f;
        target.DOFade(1f, durationIn);
        await rect.DOScale(Vector3.one, durationIn).AsyncWaitForCompletion();
    }

    public async override UniTask Off(CanvasGroup target, RectTransform container = null)
    {
        var rect = (RectTransform) target.transform;
        target.DOFade(0.2f, durationOut);
        await rect.DOScale(Vector3.zero, durationOut).AsyncWaitForCompletion();
    }
}