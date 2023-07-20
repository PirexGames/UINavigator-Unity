using System;
using UnityEngine;

namespace PirexGames.SafePadding
{

    public class SafePadding : MonoBehaviour
    {
        #region Simulations

        public enum SimDevice
        {
            None,
            iPhoneX,
            iPhoneXsMax
        }

        public static SimDevice Sim = SimDevice.None;
        protected Rect[] NSA_iPhoneX = new Rect[]
        {
            new Rect (0f, 102f / 2436f, 1f, 2202f / 2436f),  // Portrait
            new Rect (132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // Landscape
        };

        protected Rect[] NSA_iPhoneXsMax = new Rect[]
        {
        new Rect (0f, 102f / 2688f, 1f, 2454f / 2688f),  // Portrait
        new Rect (132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f)  // Landscape
        };

        #endregion

        #region Bigfoot variables
        public static Rect CurrentSA;
        public static Action OnUpdate;
        #endregion

        protected RectTransform Panel;

        protected Rect LastSafeArea = new Rect(0, 0, 0, 0);
        [SerializeField] bool ConformX = true;
        [SerializeField] bool ConformY = true;

        void Awake()
        {
            Panel = GetComponent<RectTransform>();

            if (Panel == null)
            {
                Debug.LogError("Cannot apply safe area - no RectTransform found on " + name);
                Destroy(gameObject);
            }

            Refresh();
        }

        void Update()
        {
            Refresh();
        }

        void Refresh()
        {
            Rect safeArea = GetSafeArea();

            if (safeArea != LastSafeArea)
                ApplySafeArea(safeArea);
        }

        Rect GetSafeArea()
        {
            Rect safeArea = Screen.safeArea;
#if UNITY_EDITOR
            if (Application.isEditor && Sim != SimDevice.None)
            {
                Rect nsa = new Rect(0, 0, Screen.width, Screen.height);

                switch (Sim)
                {
                    case SimDevice.iPhoneX:
                        if (Screen.height > Screen.width)  // Portrait
                            nsa = NSA_iPhoneX[0];
                        else  // Landscape
                            nsa = NSA_iPhoneX[1];
                        break;
                    case SimDevice.iPhoneXsMax:
                        if (Screen.height > Screen.width)  // Portrait
                            nsa = NSA_iPhoneXsMax[0];
                        else  // Landscape
                            nsa = NSA_iPhoneXsMax[1];
                        break;
                    default:
                        break;
                }

                safeArea = new Rect(Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width, Screen.height * nsa.height);
            }
#endif
            CurrentSA = safeArea;

            return safeArea;
        }

        protected virtual void ApplySafeArea(Rect r)
        {
            LastSafeArea = r;

            // Ignore x-axis?
            if (!ConformX)
            {
                r.x = 0;
                r.width = Screen.width;
            }

            // Ignore y-axis?
            if (!ConformY)
            {
                r.y = 0;
                r.height = Screen.height;
            }

            float widthPadding = Screen.width - r.width;
            Vector2 minPadding = new Vector2(r.position.x > 0f ? 0f : widthPadding, 0f);
            Vector2 maxPadding = new Vector2(r.position.x > 0f ? widthPadding : 0f, 0f);

            Vector2 anchorMin = r.position + minPadding;
            Vector2 anchorMax = r.position + (r.size - maxPadding);
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            Panel.anchorMin = anchorMin;
            Panel.anchorMax = anchorMax;

            OnUpdate?.Invoke();
        }
    }


}