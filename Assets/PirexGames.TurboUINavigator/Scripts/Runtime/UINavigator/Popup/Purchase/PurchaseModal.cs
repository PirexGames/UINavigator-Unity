using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PirexGames.Core.UINavigator;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PirexGames.Core.UINavigator
{
    public class PurchaseModal : EView
    {
        [Header("Header")][SerializeField] protected TextMeshProUGUI titleTxt;

        [Header("Body")][SerializeField] protected TextMeshProUGUI descriptionTxt;
        [SerializeField] protected Image itemImage;
        [SerializeField] protected TextMeshProUGUI txtAmountItem;

        [Header("Footer")][SerializeField] protected NumberStepper buyCountPack;
        [SerializeField] protected Button confirmBtn;
        [SerializeField] protected TextMeshProUGUI confirmTxt;
        [SerializeField] protected TextMeshProUGUI txtCost;
        [SerializeField] protected Image iconUnit;

        protected CallbackStatus _callbackStatus;

        protected int _buyAmountPack = 0;
        protected int _pack = 0;
        protected string _nameUnit;
        protected int _cost;

        protected override void Awake()
        {
            base.Awake();
            _callbackStatus = CallbackStatus.None;
            if (buyCountPack) buyCountPack.AddListener(OnChangeBuyCount);
            if (confirmBtn) confirmBtn.onClick.AddListener(BuyClick);
            if (buyCountPack) buyCountPack.Setup(1);
        }

        protected virtual void OnChangeBuyCount(int count)
        {
            txtAmountItem.text = String.Format("x{0}", _pack * count);
            SetupTextCost(count);
        }

        protected virtual void SetupTextCost(int count)
        {
            txtCost.text = String.Format("{0} {1}", _cost * count, _nameUnit);
        }

        protected void BuyClick()
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

        public virtual void InitButton(string buttonConfirm)
        {
            if (IsValidString(buttonConfirm))
            {
                confirmTxt.text = buttonConfirm;
            }
            else confirmBtn.gameObject.SetActive(false);
        }

        public virtual void Close()
        {
            _callbackStatus = CallbackStatus.Neutral;
        }
        private bool IsValidString(string str) => (str != string.Empty && str != "" && str != null);
    }
}