using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public abstract class AnimationTransition : ScriptableObject
{
    [SerializeField] protected float duration = .15f;
    public abstract UniTask Excute(CanvasGroup target, RectTransform container = null);
    public abstract UniTask Excute(CanvasGroup target, Vector2 position, bool isLocalPosition = false);
}