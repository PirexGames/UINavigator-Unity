using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PirexGames.Core.UINavigator
{
    public class ModalView : ModalBaseView
    {
        [Header("Modal Controls")]
        [SerializeField] protected Button[] btnsClose;

        protected override async UniTask Initialize()
        {
            await base.Initialize();
            if (btnsClose != null)
            {
                for (int i = 0; i < btnsClose.Length; i++)
                    btnsClose[i].onClick.AddListener(OnClose);
            }
        }

        private void OnDisable()
        {
            if (btnsClose == null) return;
            for (int i = 0; i < btnsClose.Length; i++)
                btnsClose[i].onClick.RemoveListener(OnClose);
        }

        protected virtual void OnClose()
        {
            this.Pop();
        }
    }
}
