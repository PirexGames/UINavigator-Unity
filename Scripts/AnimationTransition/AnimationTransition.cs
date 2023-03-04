using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public abstract class AnimationTransition : ScriptableObject
{
    [SerializeField] protected float durationIn = .15f;
    [SerializeField] protected float durationOut = .15f;
    public abstract UniTask Excute(CanvasGroup target, RectTransform container = null);
}