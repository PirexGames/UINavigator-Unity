using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

namespace PirexGames.UI.Base
{
    public class Tooltip : EView
    {
        [SerializeField] private TextMeshProUGUI contentTxt;
        public async void Init(string content, Vector2 position, bool isLocalPosition = false)
        {
            if (contentTxt)
                contentTxt.text = content;
            await UniTask.NextFrame();
            await OnShow(position, isLocalPosition);
        }
    }
}