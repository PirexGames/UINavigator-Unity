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
        void Start()
        {
            pushBtn.onClick.AddListener(Push);
            popBtn.onClick.AddListener(Pop);
            alertBtn.onClick.AddListener(Alert);
        }
        private void Push()
        {
            ScreenContainer.Push("ScreenDefault");
        }
        private void Pop()
        {
            ScreenContainer.Pop();
        }

        private void Alert()
        {
            ModalContainer.Alert(
                "ModalDefault",
                "Hãy cứ vui tươi và lạc quan lên em ơi",
                "Aloo xin chao cac bạn",
                CallBackAlert,
                "Dồng ý",
                "Cút");
        }

        private void CallBackAlert(CallbackStatus callbackStatus)
        {
            Debug.Log(callbackStatus);
        }
    }
}