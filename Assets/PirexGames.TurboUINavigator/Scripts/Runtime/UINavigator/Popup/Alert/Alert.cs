using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Events;

namespace PirexGames.Core.UINavigator
{
    public class Alert : EView
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

        private UniTaskCompletionSource<CallbackStatus> _callbackStatus;
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
            _callbackStatus = new();
            if (!string.IsNullOrEmpty(title))
                if (titleTxt)
                    titleTxt.text = title;
            InitButton(buttonPositive, buttonNegative, buttonNeutral);
            await UniTask.NextFrame();
            await OnShow();
            await _callbackStatus.Task;
            await OnHide();
            return _callbackStatus.GetResult(new short());
        }

        public async UniTask<CallbackStatus> Init
        (
            string title, string content,
            string buttonPositive, string buttonNegative, string buttonNeutral)
        {
            _callbackStatus = new();
            if (!string.IsNullOrEmpty(title))
                if (titleTxt)
                    titleTxt.text = title;
            if (!string.IsNullOrEmpty(content))
                contentTxt.text = content;
            InitButton(buttonPositive, buttonNegative, buttonNeutral);
            await OnShow();
            await UniTask.NextFrame();
            await _callbackStatus.Task;

            await OnHide();
            return _callbackStatus.GetResult(new short());
        }

        public void InitButton(string txtPositive, string txtNegative, string txtNeutral)
        {
            if (!string.IsNullOrEmpty(txtPositive))
                positiveTxt.text = txtPositive;
            else positiveBtn.gameObject.SetActive(false);

            if (!string.IsNullOrEmpty(txtNegative))
                negativeTxt.text = txtNegative;
            else negativeBtn.gameObject.SetActive(false);

            if (!string.IsNullOrEmpty(txtNeutral))
                neutralTxt.text = txtNeutral;
            else neutralBtn.gameObject.SetActive(false);

            if (string.IsNullOrEmpty(txtPositive + txtNegative + txtNeutral)) positiveBtn.gameObject.SetActive(true);
        }
        protected virtual void PositiveClick()
        {
            _callbackStatus.TrySetResult(CallbackStatus.Positive);
        }

        protected virtual void NegativeClick()
        {
            _callbackStatus.TrySetResult(CallbackStatus.Negative);
        }

        protected virtual void NeutralClick()
        {
            _callbackStatus.TrySetResult(CallbackStatus.Neutral);
        }
    }
}