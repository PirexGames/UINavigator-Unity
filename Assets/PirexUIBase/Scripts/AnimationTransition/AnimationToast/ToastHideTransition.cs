using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "ToastHideTransition", menuName = "Pirex.UI.Base/Animation/ToastHideTransitionSO")]
public class ToastHideTransition : AnimationTransition
{
    [SerializeField] private float sizeTo = 0f;
    public async override UniTask Excute(CanvasGroup target, RectTransform container = null)
    {
        var rect = (RectTransform)target.transform;
        target.DOFade(0.2f, durationOut);
        await rect.DOScale(Vector3.one * sizeTo, durationOut).AsyncWaitForCompletion();
    }
}