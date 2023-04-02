using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

namespace Pirex.UI.Base
{
    public class Modal : UIView
    {
        [SerializeField] protected Button closeBtn;
        [SerializeField] protected TextMeshProUGUI titleTxt;
        protected bool isClose = false;
        
        protected override void Awake() {
            base.Awake();
            if (closeBtn) 
                closeBtn.onClick.AddListener(OnClose);
        }
        public async UniTask Init
        (
            string title
        )
        {
            isClose = false;
            if (titleTxt.text is not null)
                titleTxt.text = title;
            await OnPushEnter();
            await UniTask.WaitUntil(() => isClose == true);
            await UniTask.NextFrame();
            await OnPopExit();
        }

        private void OnClose()
        {
            isClose = true;
        }
    }
}
