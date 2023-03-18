using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Pirex.UI.Base
{
    public class Screen : UIView
    {
        [SerializeField] protected bool isHidenOnPushOther = true;

        [SerializeField] protected Button btnBack;
        protected override void Awake()
        {
            base.Awake();
            if (btnBack != null)
                btnBack.onClick.AddListener(OnBack);
        }
        public void OnPushOtherScreen()
        {
            gameObject.SetActive(!isHidenOnPushOther);
        }

        private void OnBack()
        {
            ScreenContainer.Pop().Forget();
        }
    }
}
