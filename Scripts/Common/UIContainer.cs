using UnityEngine;

public abstract class UIContainer : MonoBehaviour
{
    public static Transform transformContainer;
    public static RectTransform rectContainer;
    protected abstract void Awake();
}