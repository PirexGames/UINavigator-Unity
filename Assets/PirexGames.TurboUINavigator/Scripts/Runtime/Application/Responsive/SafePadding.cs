using UnityEngine;
using System;

namespace PirexGames.SafePadding
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-999)]
    public class SafePadding : MonoBehaviour
    {
        public enum SimDevice
        {
            None,
            iPhoneX,
            iPhoneXsMax
        }

        public static SimDevice Sim = SimDevice.None;

        // Giả lập SafeArea trong Editor
        private readonly Rect[] SimSafeAreas_iPhoneX = new Rect[]
        {
            new Rect(0f, 102f / 2436f, 1f, 2202f / 2436f),     // Portrait
            new Rect(132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // Landscape
        };

        private readonly Rect[] SimSafeAreas_iPhoneXsMax = new Rect[]
        {
            new Rect(0f, 102f / 2688f, 1f, 2454f / 2688f),     // Portrait
            new Rect(132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f)  // Landscape
        };

        public static Rect CurrentSafeArea { get; private set; }
        public static Action OnUpdate;

        [SerializeField] private bool ConformX = true;
        [SerializeField] private bool ConformY = true;

        private RectTransform panel;
        private Rect lastSafeArea = new Rect(0, 0, 0, 0);

        void Awake()
        {
            panel = GetComponent<RectTransform>();

            if (panel == null)
            {
                Destroy(this);
                return;
            }

            ApplySafeArea(GetSafeArea());
        }

        void Update()
        {
            Rect current = GetSafeArea();
            if (current != lastSafeArea)
                ApplySafeArea(current);
        }

        private Rect GetSafeArea()
        {
            Rect safeArea = Screen.safeArea;

#if UNITY_EDITOR
            if (Application.isEditor && Sim != SimDevice.None)
            {
                Rect simulated = new Rect(0, 0, 1, 1); // fallback
                if (Sim == SimDevice.iPhoneX)
                    simulated = (Screen.height > Screen.width) ? SimSafeAreas_iPhoneX[0] : SimSafeAreas_iPhoneX[1];
                else if (Sim == SimDevice.iPhoneXsMax)
                    simulated = (Screen.height > Screen.width) ? SimSafeAreas_iPhoneXsMax[0] : SimSafeAreas_iPhoneXsMax[1];

                safeArea = new Rect(
                    simulated.x * Screen.width,
                    simulated.y * Screen.height,
                    simulated.width * Screen.width,
                    simulated.height * Screen.height
                );
            }
#endif

            CurrentSafeArea = safeArea;
            return safeArea;
        }

        private void ApplySafeArea(Rect r)
        {
            lastSafeArea = r;

#if UNITY_ANDROID
            // Xử lý cân bằng padding 2 bên trong chế độ landscape
            if (Screen.width > Screen.height && ConformX)
            {
                float leftPadding = r.x;
                float rightPadding = Screen.width - (r.x + r.width);
                float paddingDiff = Mathf.Abs(leftPadding - rightPadding);

                // Nếu chỉ lệch 1 bên rõ rệt, ta sẽ cân lại để đều 2 bên
                if (paddingDiff > 50f) // ngưỡng 50 pixels
                {
                    float maxPadding = Mathf.Max(leftPadding, rightPadding);
                    r.x = maxPadding;
                    r.width = Screen.width - (2f * maxPadding);
                }
            }
#endif

            if (!ConformX)
            {
                r.x = 0;
                r.width = Screen.width;
            }

            if (!ConformY)
            {
                r.y = 0;
                r.height = Screen.height;
            }

            Vector2 anchorMin = new Vector2(r.x / Screen.width, r.y / Screen.height);
            Vector2 anchorMax = new Vector2((r.x + r.width) / Screen.width, (r.y + r.height) / Screen.height);

            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;

            OnUpdate?.Invoke();
        }
    }
}
