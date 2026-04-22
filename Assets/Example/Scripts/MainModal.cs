using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PirexGames.Core.UINavigator;
using UnityEngine;
using UnityEngine.UI;

public class MainModal : ModalView
{
    [SerializeField] private Button btnOpenSubModal;

    private void Start()
    {
        btnOpenSubModal.onClick.AddListener(OnOpenSubModal);
    }

    private async void OnOpenSubModal()
    {
        var modal = await UINavigator.Modals.Push<ModalView>("ModalDefault");
    }
}
