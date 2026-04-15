using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PirexGames.Core.UINavigator
{
    public class MainScreen : Screen
    {
        [SerializeField] private Button pushBtn;
        [SerializeField] private Button alertBtn;
        [SerializeField] private Button modalBtn;
        [SerializeField] private Button toastBtn;
        [SerializeField] private Button overloadToastBtn;
        [SerializeField] private Button tooltipBtn;
        [SerializeField] private Transform objectTooltipShow;

        [SerializeField] private TooltipPosition tooltipPosition;
        [SerializeField] private Button purchaseBtn;

        void Start()
        {
            pushBtn.onClick.AddListener(Push);
            alertBtn.onClick.AddListener(Alert);
            modalBtn.onClick.AddListener(ShowModalAsync);
            toastBtn.onClick.AddListener(Toast);
            tooltipBtn.onClick.AddListener(TooltipShow);
            overloadToastBtn.onClick.AddListener(Overload);
            // Purchase logic requires PurchaseModalContainer which we'll assume is updated or similar to Alert
            purchaseBtn.onClick.AddListener(Purchase);
        }

        public void Push()
        {
            // NEW: Using UINavigator hub
            UINavigator.Screens.Push<Screen>("ScreenDefault").Forget();
        }

        private void Pop()
        {
            UINavigator.Screens.Pop().Forget();
        }

        private async void ShowModalAsync()
        {
            // NEW: Typed push returns the modal instance
            var modal = await UINavigator.Modals.Push<ModalBaseView>("ModalDefault", closeOnClickBackdrop: false);
        }

        private void Alert()
        {
            // NEW: Fully synchronized instance-based call
            UINavigator.Alerts.Show(
                "AlertDefault", 
                "Notice", 
                "Do you want to confirm this action?", 
                (status) => {
                    if (status == CallbackStatus.Positive)
                        Alert();
                },
                buttonPositive: "Yes",
                buttonNegative: "No",
                closeOnClickBackdrop: false
            ).Forget();
        }

        private void CallBackAlert(CallbackStatus callbackStatus)
        {
            if (callbackStatus == CallbackStatus.Positive)
                Alert();
        }

        private void Toast()
        {
            // NEW: Using generic hub for toasts
            UINavigator.MonoToasts.Show<ToastView>("ToastDefault", "This is the toast", 2f).Forget();
        }

        private void Overload()
        {
            // NEW: Named container example (if assigned)
            UINavigator.MultipleToasts.Show<ToastView>("ToastDefault", "This is the overload toast", 2f).Forget();
        }

        private void TooltipShow()
        {
            // NEW: Tooltip positioning integrated into hub
            UINavigator.Tooltips.Show<TooltipBaseAutoView>("TooltipDefault", $"This is the tooltip {tooltipPosition}",
                (RectTransform)objectTooltipShow, tooltipPosition, Vector2.zero).Forget();
        }

        public Sprite demoTexture2d;
        public Sprite demoUnit;

        private void Purchase()
        {
            // Assuming PurchaseModal has been similarly refactored or kept compatible
            // For now, let's keep it as is if the container still exists or map it
            Debug.Log("Purchase clicked");
        }
    }
}