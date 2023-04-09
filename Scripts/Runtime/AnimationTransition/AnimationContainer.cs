using UnityEngine;

[System.Serializable]
public class AnimationContainerCView
{
    public AnimationTransition PushEnterAnimation;
    public AnimationTransition PushExitAnimation;
    public AnimationTransition PopEnterAnimation;
    public AnimationTransition PopExitAnimation;
}

[System.Serializable]
public class AnimationContainerEView
{
    public AnimationTransition ShowAnimation;
    public AnimationTransition HideAnimation;
}