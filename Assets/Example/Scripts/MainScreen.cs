using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PirexGames.Core.UINavigator
{
    /// <summary>
    /// Example of a Main Screen using the Turbo UINavigator Hub.
    /// This script demonstrates the most common UI operations.
    /// </summary>
    public class MainScreen : ScreenView
    {
        [Header("Screens & Modals")]
        [SerializeField] private Button pushScreenBtn;
        [SerializeField] private Button showModalBtn;
        [SerializeField] private Button showAlertBtn;

        [Header("Toasts & Tooltips")]
        [SerializeField] private Button toastBtn;
        [SerializeField] private Button tooltipBtn;
        [SerializeField] private RectTransform tooltipTarget;

        [Header("Settings")]
        [SerializeField] private TooltipPosition tooltipPos = TooltipPosition.Auto;

        protected override void Awake()
        {
            base.Awake();
            
            // Map buttons to operations
            pushScreenBtn.onClick.AddListener(OnPushClicked);
            showModalBtn.onClick.AddListener(OnModalClicked);
            showAlertBtn.onClick.AddListener(OnAlertClicked);
            toastBtn.onClick.AddListener(OnToastClicked);
            tooltipBtn.onClick.AddListener(OnTooltipClicked);
        }

        /// <summary>
        /// Demonstrates pushing a new screen onto the stack.
        /// </summary>
        private void OnPushClicked()
        {
            // Use the Hub to push a screen by its Addressable key
            UINavigatorHub.MainScreen.Push<ScreenView>("ScreenDefault").Forget();
        }

        /// <summary>
        /// Demonstrates showing a modal and awaiting its result.
        /// </summary>
        private async void OnModalClicked()
        {
            // Modals can be awaited to get the instance for further setup
            var modal = await UINavigator.Modals.Push<ModalView>("ModalDefault");
            
            if (modal != null)
            {
                Debug.Log($"Modal {modal.name} opened successfully!");
            }
        }

        /// <summary>
        /// Demonstrates the simplified Alert system with callbacks.
        /// </summary>
        private void OnAlertClicked()
        {
            UINavigator.Alerts.Show(
                "AlertDefault",
                "System Notice",
                "This is a high-performance alert box. Do you like it?",
                (status) =>
                {
                    if (status == CallbackStatus.Positive)
                    {
                        OnAlertClicked();
                        Debug.Log("User clicked YES");
                    }
                    else
                        Debug.Log("User clicked NO");
                },
                buttonPositive: "YES",
                buttonNegative: "NO"
            ).Forget();
        }

        /// <summary>
        /// Demonstrates zero-allocation toast messaging.
        /// </summary>
        private void OnToastClicked()
        {
            // MonoToasts reuse the same container to show one toast at a time
            UINavigator.MonoToasts.Show<ToastView>("ToastDefault", "Action successful!", 2f).Forget();
        }

        /// <summary>
        /// Demonstrates smart, boundary-aware tooltips.
        /// </summary>
        private void OnTooltipClicked()
        {
            UINavigator.Tooltips.Show<TooltipBaseAutoView>(
                "TooltipDefault",
                "I am a smart tooltip that knows screen boundaries!",
                tooltipTarget,
                tooltipPos
            ).Forget();
        }
    }
}
