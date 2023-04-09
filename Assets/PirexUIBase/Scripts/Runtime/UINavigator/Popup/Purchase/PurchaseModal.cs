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
        [SerializeField] private TextMeshProUGUI txtAmountItem;

        [Header("Footer")][SerializeField] private NumberStepper buyCountPack;
        [SerializeField] private Button confirmBtn;
        [SerializeField] private TextMeshProUGUI confirmTxt;
        [SerializeField] private TextMeshProUGUI txtCost;
        [SerializeField] private Image iconUnit;

        private CallbackStatus _callbackStatus;

        private int _buyAmountPack = 0;
        private int _pack = 0;
        private string _nameUnit;
        private int _cost;

        protected override void Awake()
        {
            base.Awake();
            _callbackStatus = CallbackStatus.None;
            if (buyCountPack) buyCountPack.AddListener(OnChangeBuyCount);
            if (confirmBtn) confirmBtn.onClick.AddListener(BuyClick);
            if (buyCountPack) buyCountPack.Setup(1);
        }

        private void OnChangeBuyCount(int count)
        {
            txtAmountItem.text = String.Format("x{0}", _pack * count);
            txtCost.text = String.Format("{0} {1}", _cost * count, _nameUnit);
        }

        private void BuyClick()
        {
            _buyAmountPack = buyCountPack.Value;
            _callbackStatus = CallbackStatus.Positive;
        }

        public async UniTask Init
        (
            string id,
            string title, string description, Sprite sprite,
            int pack, int cost, Sprite unitIcon, string nameUnit,
            string buttonConfirm, Action<int, string> callback,
            int maxPack = 1
        )
        {
            if (titleTxt.text is not null)
                titleTxt.text = title;
            descriptionTxt.text = description;
            itemImage.sprite = sprite;
            iconUnit.sprite = unitIcon;
            _pack = pack;
            _nameUnit = nameUnit;
            _cost = cost;
            buyCountPack.SetMaxValue(maxPack);
            if (maxPack == 1) buyCountPack.gameObject.SetActive(false);
            OnChangeBuyCount(1);
            InitButton(buttonConfirm);
            await UniTask.NextFrame();
            await OnShow();
            await UniTask.WaitUntil(() => _callbackStatus != CallbackStatus.None);
            if (_callbackStatus == CallbackStatus.Positive)
                callback?.Invoke(_buyAmountPack, id);
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