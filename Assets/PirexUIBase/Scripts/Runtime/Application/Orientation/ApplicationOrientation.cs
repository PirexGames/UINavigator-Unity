using UnityEngine;

public class ApplicationOrientation : MonoBehaviour
{
    [SerializeField] private bool horizontalOrientation = true;
    private void Update()
    {
        if (horizontalOrientation)
        {
            if ((Input.deviceOrientation == DeviceOrientation.LandscapeLeft) && (Screen.orientation != ScreenOrientation.LandscapeLeft))
                Screen.orientation = ScreenOrientation.LandscapeLeft;

            if ((Input.deviceOrientation == DeviceOrientation.LandscapeRight) && (Screen.orientation != ScreenOrientation.LandscapeRight))
                Screen.orientation = ScreenOrientation.LandscapeRight;
        }

    }
}