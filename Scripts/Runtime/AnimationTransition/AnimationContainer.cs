using UnityEngine;

[System.Serializable]
public class AnimationContainer
{
    public AnimationTransition PushEnterAnimation;
    public AnimationTransition PushExitAnimation;
    public AnimationTransition PopEnterAnimation;
    public AnimationTransition PopExitAnimation;
}

[System.Serializable]
public class AnimationTab
{
    public AnimationTransition ShowAnimation;
    public AnimationTransition HideAnimation;
}