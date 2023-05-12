using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Pirex.UI.Base
{
    public class Screen : ScreenView
    {

        [SerializeField] protected Button btnBack;
        protected override void Awake()
        {
            base.Awake();
            if (btnBack != null)
                btnBack.onClick.AddListener(OnBack);
        }
        protected async override UniTask Initialize()
        {
            await UniTask.Delay(2000);
            await base.Initialize();
        }

        private void OnBack()
        {
            ScreenContainer.Pop().Forget();
        }
    }
}
