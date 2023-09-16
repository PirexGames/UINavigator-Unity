using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

namespace PirexGames.UI.Base
{
    public class Modal : EView
    {
        [SerializeField] protected Button closeBtn;
        [SerializeField] protected TextMeshProUGUI titleTxt;
        protected bool isClose = false;

        protected override void Awake()
        {
            base.Awake();
        }

        protected async override UniTask Initialize()
        {
            await base.Initialize();
            if (closeBtn)
                closeBtn.onClick.AddListener(OnClose);
        }

        public async UniTask Init
        (
            string title
        )
        {
            isClose = false;
            if (titleTxt.text is not null && !string.IsNullOrEmpty(title))
                titleTxt.text = title;
            await UniTask.NextFrame();
            await OnShow();
        }

        protected virtual void OnClose()
        {
            ModalContainer.Pop().Forget();
        }
    }
}
