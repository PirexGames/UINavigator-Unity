using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pirex.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Pirex.UI.Base
{
    public class PurchaseModal : EView
    {
        [Header("Header")][SerializeField] private TextMeshProUGUI titleTxt;

        [Header("Body")][SerializeField] private TextMeshProUGUI descriptionTxt;
        [SerializeField] private Image itemImage;

        [Header("Footer")][SerializeField] private NumberStepper buyCount;
        [SerializeField] private Button confirmBtn;
        [SerializeField] private TextMeshProUGUI confirmTxt;

        private CallbackStatus _callbackStatus;

        private int _buyAmount = 0;

        protected override void Awake()
        {
            base.Awake();
            _callbackStatus = CallbackStatus.None;
            if (confirmBtn) confirmBtn.onClick.AddListener(BuyClick);
            buyCount.Setup(1);
        }

        private void BuyClick()
        {
            _buyAmount = buyCount.Value;
            _callbackStatus = CallbackStatus.Positive;
        }

        public async UniTask Init
        (
            string id,
            string title, string description, Sprite sprite,
            string buttonConfirm, Action<int, string> callback
        )
        {
            if (titleTxt.text is not null)
                titleTxt.text = title;
            descriptionTxt.text = description;
            itemImage.sprite = sprite;
            InitButton(buttonConfirm);
            await UniTask.NextFrame();
            await OnShow();
            await UniTask.WaitUntil(() => _callbackStatus != CallbackStatus.None);
            if (_callbackStatus == CallbackStatus.Positive)
                callback?.Invoke(_buyAmount, id);
            await OnHide();
        }

        public void InitButton(string buttonConfirm)
        {
            if (IsValidString(buttonConfirm))
            {
                confirmTxt.text = buttonConfirm;
            }
            else confirmBtn.gameObject.SetActive(false);
        }

        public void Close()
        {
            _callbackStatus = CallbackStatus.Neutral;
        }
        private bool IsValidString(string str) => (str != string.Empty && str != "" && str != null);
    }
}