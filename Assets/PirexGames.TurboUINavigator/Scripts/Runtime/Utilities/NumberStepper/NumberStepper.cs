using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberStepper : MonoBehaviour
{
    [SerializeField] private Button addButton;
    [SerializeField] private Button minusButton;

    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private int step = 1;
    [SerializeField] private int minValue = 0;
    [SerializeField] private int maxValue = 100;

    private Action<int> _onChangeValue;
    private int _count = 0;
    public int Value => _count;

    private void Awake()
    {
        SetValue(minValue);
        addButton.onClick.AddListener(delegate { OnChangeValue(1); });
        minusButton.onClick.AddListener(delegate { OnChangeValue(-1); });
    }

    public void Setup(int value) => SetValue(value);
    private void OnDisable() => SetValue(minValue);
    private void OnChangeValue(int value)
    {
        SetValue(value * step + _count);
        _onChangeValue?.Invoke(_count);
    }

    private void SetValue(int value)
    {
        _count = Mathf.Clamp(value, minValue, maxValue);
        numberText.text = _count.ToString();
    }

    public void AddListener(Action<int> action)
    {
        _onChangeValue += action;
    }

    public void SetMaxValue(int value)
    {
        if (value <= minValue) maxValue = minValue;
        else maxValue = value;
    }
}