using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace PirexGames.Core.UINavigator
{
    public class Screen : ScreenView
    {

        [SerializeField] protected Button[] btnsBack;
        protected override void Awake()
        {
            base.Awake();
            if (btnsBack != null)
            {
                foreach (var item in btnsBack)
                    item.onClick.AddListener(OnBack);
            }
        }
        protected override async UniTask Initialize()
        {
            await base.Initialize();
        }

        protected virtual void OnBack()
        {
            ScreenContainer.Pop().Forget();
        }
    }
}
