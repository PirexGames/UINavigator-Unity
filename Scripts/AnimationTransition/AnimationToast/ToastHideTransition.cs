using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "ToastHideTransition", menuName = "Pirex.UI.Base/Animation/ToastHideTransitionSO")]
public class ToastHideTransition : AnimationTransition
{
    [SerializeField] private float sizeTo = 0f;
    public async override UniTask Excute(CanvasGroup target, RectTransform container = null)
    {
        var rect = (RectTransform)target.transform;
        target.DOFade(0.2f, duration);
        await rect.DOScale(Vector3.one * sizeTo, duration).AsyncWaitForCompletion();
    }

    public override async UniTask Excute(CanvasGroup target, Vector2 position, bool isLocalPosition = false)
    {
        await UniTask.CompletedTask;
    }
}