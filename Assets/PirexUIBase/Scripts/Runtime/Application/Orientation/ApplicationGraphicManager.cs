using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pirex.UI.Base
{
    public class ApplicationGraphicManager : MonoBehaviour
    {
        [SerializeField] private FPSEnum fpsEnum;

        void Awake()
        {
            SetFPS(fpsEnum);
        }

        public static void SetFPS(FPSEnum fps)
        {
            Application.targetFrameRate = (int)fps;
        }

    }

    public enum FPSEnum : int
    {
        FPS30 = 30,
        FPS60 = 60,
        FPS90 = 90,
        FPS120 = 120,
    }

}
