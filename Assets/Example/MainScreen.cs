using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pirex.UI.Base
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
        [SerializeField] private OverloadToastContainer overloadToastContainer;

        [SerializeField] private TooltipPosition tooltipPosition;

        // [SerializeField] private 
        [SerializeField] private Button purchaseBtn;

        void Start()
        {
            pushBtn.onClick.AddListener(Push);
            alertBtn.onClick.AddListener(Alert);
            modalBtn.onClick.AddListener(ShowModal);
            toastBtn.onClick.AddListener(Toast);
            tooltipBtn.onClick.AddListener(TooltipShow);
            overloadToastBtn.onClick.AddListener(Overload);
            purchaseBtn.onClick.AddListener(Purchase);
        }


        public void Push()
        {
            ScreenContainer.Push("ScreenDefault").Forget();
        }

        private void Pop()
        {
            ScreenContainer.Pop().Forget();
        }

        private void ShowModal()
        {
            ModalContainer.Show("ModalDefault", "Settings").Forget();
        }

        private void Alert()
        {
            AlertContainer.Alert(
                "AlertDefault",
                "This is the title",
                "This is the content alert",
                CallBackAlert,
                buttonPositive: "OK", buttonNegative: "Cancel");
        }

        private void CallBackAlert(CallbackStatus callbackStatus)
        {
            if (callbackStatus == CallbackStatus.Positive)
                Alert();
        }

        private void Toast()
        {
            MonoToast.ShowToast("ToastDefault", "This is the toast", 2f).Forget();
        }

        private void Overload()
        {
            overloadToastContainer.ShowToast("ToastDefault", "This is the overload toast", 2f).Forget();
        }

        private void TooltipShow()
        {
            TooltipContainer.Show("TooltipDefault", $"This is the tooltip {tooltipPosition.ToString()}",
                (RectTransform)objectTooltipShow, tooltipPosition).Forget();
        }

        public Sprite demoTexture2d;
        public Sprite demoUnit;

        public void Callback(int amount, string id)
        {
            AlertContainer.Alert(
               "AlertDefault",
               "Purchase Alert",
               $"You get {amount} with the product {id}", buttonNeutral: "Yay!");
        }

        private void Purchase()
        {
            PurchaseModalContainer.Show(
                "PurchaseDefault",
                "id234324",
                "Purchase title",
                demoUnit,
                7,
                19,
                "PxPoint",
                "demo scription",
                demoTexture2d,
                Callback,
                "Buy", maxPack: 6);
        }
    }
}