using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "ToastShowTransition", menuName = "Pirex.UI.Base/Animation/ToastShowTransitionSO")]
public class ToastShowTransition : AnimationTransition
{
    [SerializeField] private float sizeFrom = 0f;
    public async override UniTask Excute(CanvasGroup target, RectTransform container = null)
    {
        var rect = (RectTransform)target.transform;
        rect.localScale = Vector3.one * sizeFrom;
        target.alpha = 0f;
        target.DOFade(1f, durationIn);
        await rect.DOScale(Vector3.one, durationIn).AsyncWaitForCompletion();
    }
}