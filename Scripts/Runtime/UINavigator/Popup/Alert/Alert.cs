using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Events;

namespace Pirex.UI.Base
{
    public class Alert : UIView
    {
        [Header("Header")]
        [SerializeField] protected TextMeshProUGUI titleTxt;
        [Header("Body")]
        [SerializeField] protected TextMeshProUGUI contentTxt;

        [Header("Bottom")]
        [SerializeField] protected Button positiveBtn;
        [SerializeField] protected Button negativeBtn;
        [SerializeField] protected Button neutralBtn;

        [SerializeField] protected TextMeshProUGUI positiveTxt;
        [SerializeField] protected TextMeshProUGUI negativeTxt;
        [SerializeField] protected TextMeshProUGUI neutralTxt;

        private CallbackStatus _callbackStatus;
        protected CanvasGroup modalCG;

        protected override void Awake()
        {
            base.Awake();
            if (positiveBtn)
                positiveBtn.onClick.AddListener(PositiveClick);
            if (negativeBtn)
                negativeBtn.onClick.AddListener(NegativeClick);
            if (neutralBtn)
                neutralBtn.onClick.AddListener(NeutralClick);
        }

        public async UniTask<CallbackStatus> Init
        (
            string title, GameObject view,
            string buttonPositive, string buttonNegative, string buttonNeutral
        )
        {
            _callbackStatus = CallbackStatus.None;
            if (titleTxt.text is not null)
                titleTxt.text = title;
            InitButton(buttonPositive, buttonNegative, buttonNeutral);
            await UniTask.NextFrame();
            await OnPushEnter();
            await UniTask.WaitUntil(() => _callbackStatus != CallbackStatus.None);
            await OnPopExit();
            return _callbackStatus;
        }

        public async UniTask<CallbackStatus> Init
        (
            string title, string content,
            string buttonPositive, string buttonNegative, string buttonNeutral)
        {
            _callbackStatus = CallbackStatus.None;
            if (titleTxt.text is not null)
                titleTxt.text = title;
            contentTxt.text = content;
            InitButton(buttonPositive, buttonNegative, buttonNeutral);
            await OnPushEnter();
            await UniTask.NextFrame();
            await UniTask.WaitUntil(() => _callbackStatus != CallbackStatus.None);
            await OnPopExit();
            return _callbackStatus;
        }

        public void InitButton(string txtPositive, string txtNegative, string txtNeutral)
        {
            if (IsValidString(txtPositive))
            {
                positiveTxt.text = txtPositive;
            }
            else positiveBtn.gameObject.SetActive(false);


            if (IsValidString(txtNegative))
            {
                negativeTxt.text = txtNegative;
            }
            else negativeBtn.gameObject.SetActive(false);

            if (IsValidString(txtNeutral))
            {
                neutralTxt.text = txtNeutral;
            }
            else neutralBtn.gameObject.SetActive(false);
        }
        private bool IsValidString(string str) => (str != string.Empty && str != "" && str != null);
        private void PositiveClick()
        {
            _callbackStatus = CallbackStatus.Positive;
        }

        private void NegativeClick()
        {
            _callbackStatus = CallbackStatus.Negative;
        }

        private void NeutralClick()
        {
            _callbackStatus = CallbackStatus.Neutral;
        }
    }
}