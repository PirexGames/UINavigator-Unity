using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pirex.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PurchaseModal : UIView
{
    [Header("Header")] [SerializeField] private TextMeshProUGUI titleTxt;

    [Header("Body")] [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private RawImage itemImage;

    [Header("Footer")] [SerializeField] private NumberStepper buyCount;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private TextMeshProUGUI confirmTxt;
    
    private byte _checkAmount;

    private int _buyAmount = 0;

    protected override void Awake()
    {
        base.Awake();
        if (confirmBtn) confirmBtn.onClick.AddListener(BuyClick);
    }

    private void OnEnable()
    {
        _checkAmount = 0;
    }

    private void BuyClick()
    {
        _buyAmount = buyCount.Value;
        _checkAmount = 1;
    }

    public async UniTask<int> Init
    (
        string title, string description, Texture2D texture2D,
        string buttonConfirm
    )
    {
        if (titleTxt.text is not null)
            titleTxt.text = title;
        descriptionTxt.text = description;
        itemImage.texture = texture2D;
        InitButton(buttonConfirm);
        await UniTask.NextFrame();
        await OnShow();
        await UniTask.WaitUntil(() => _checkAmount != 0);
        await OnHide();
        return _buyAmount;
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
        _checkAmount = 2;
    }
    private bool IsValidString(string str) => (str != string.Empty && str != "" && str != null);
}