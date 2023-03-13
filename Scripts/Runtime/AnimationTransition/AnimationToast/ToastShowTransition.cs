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
        target.DOFade(1f, duration);
        await rect.DOScale(Vector3.one, duration).AsyncWaitForCompletion();
    }

    public override async UniTask Excute(CanvasGroup target, Vector2 position, bool isLocalPosition = false)
    {
        var rect = (RectTransform)target.transform;
        rect.localScale = Vector3.one * sizeFrom;
        if (isLocalPosition)
            target.transform.localPosition = position;
        else
            target.transform.position = position;
        await UniTask.NextFrame();
        target.alpha = 0f;
        target.DOFade(1f, duration);
        await rect.DOScale(Vector3.one, duration).AsyncWaitForCompletion();
    }
}