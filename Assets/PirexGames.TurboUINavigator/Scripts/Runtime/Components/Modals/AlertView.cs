using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace PirexGames.Core.UINavigator
{
    public enum CallbackStatus { Positive, Negative, Neutral }

    public class AlertView : ModalBaseView
    {
        [SerializeField] protected TMPro.TextMeshProUGUI titleTxt;
        [SerializeField] protected TMPro.TextMeshProUGUI contentTxt;
        [SerializeField] protected Button btnPositive;
        [SerializeField] protected Button btnNegative;
        [SerializeField] protected Button btnNeutral;
        [SerializeField] protected TMPro.TextMeshProUGUI txtPositive;
        [SerializeField] protected TMPro.TextMeshProUGUI txtNegative;
        [SerializeField] protected TMPro.TextMeshProUGUI txtNeutral;

        private Action<CallbackStatus> _callback;

        internal virtual void Init(string title, string content, Action<CallbackStatus> callback = null, 
            string btnPos = "", string btnNeg = "", string btnNeu = "")
        {
            if (titleTxt) titleTxt.text = title;
            if (contentTxt) contentTxt.text = content;
            _callback = callback;

            SetupButton(btnPositive, txtPositive, btnPos, CallbackStatus.Positive);
            SetupButton(btnNegative, txtNegative, btnNeg, CallbackStatus.Negative);
            SetupButton(btnNeutral, txtNeutral, btnNeu, CallbackStatus.Neutral);
        }

        private void SetupButton(Button btn, TMPro.TextMeshProUGUI label, string text, CallbackStatus status)
        {
            if (btn == null) return;
            bool active = !string.IsNullOrEmpty(text);
            btn.gameObject.SetActive(active);
            if (active)
            {
                if (label) label.text = text;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    _callback?.Invoke(status);
                    UINavigator.Alerts.Pop(this).Forget();
                });
            }
        }
    }
}
