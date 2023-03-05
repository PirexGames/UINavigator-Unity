using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pirex.UI.Base
{
    public class PushScreen : MonoBehaviour
    {
        [SerializeField] private Button pushBtn;
        [SerializeField] private Button popBtn;
        [SerializeField] private Button alertBtn;
        [SerializeField] private Button toastBtn;
        [SerializeField] private Button overloadToastBtn;
        [SerializeField] private Button tooltipBtn;
        [SerializeField] private Transform objectTooltipShow;
        [SerializeField] private OverloadToastContainer overloadToastContainer;
        [SerializeField] private TooltipPosition tooltipPosition;
        void Start()
        {
            pushBtn.onClick.AddListener(Push);
            popBtn.onClick.AddListener(Pop);
            alertBtn.onClick.AddListener(Alert);
            toastBtn.onClick.AddListener(Toast);
            tooltipBtn.onClick.AddListener(TooltipShow);
            overloadToastBtn.onClick.AddListener(Overload);
        }

        public void Push()
        {
            ScreenContainer.Push("ScreenDefault");
        }

        private void Pop()
        {
            ScreenContainer.Pop();
        }

        private void Alert()
        {
            AlertContainer.Alert(
                "ModalDefault",
                "Hãy cứ vui tươi và lạc quan lên em ơi",
                "Aloo xin chao cac bạn",
                CallBackAlert,
                buttonNeutral: "DFDKFJ");
        }

        private void CallBackAlert(CallbackStatus callbackStatus)
        {
            Debug.Log(callbackStatus);
        }

        private void Toast()
        {
            MonoToast.ShowToast("ToastDefault", "Cút mẹ mày đi", 2f);
        }

        private void Overload()
        {
            overloadToastContainer.ShowToast("ToastDefault", "Cút mẹ mày đi", 2f);
        }

        private void TooltipShow()
        {
            TooltipContainer.Show("TooltipDefault", "Cái này là Tooltiop giải thích cái gì đó", (RectTransform)objectTooltipShow, tooltipPosition);
        }
    }
}