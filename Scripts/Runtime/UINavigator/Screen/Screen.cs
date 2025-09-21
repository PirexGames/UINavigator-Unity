using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace PirexGames.Core.UINavigator
{
    public class Screen : ScreenView
    {
        [SerializeField] protected Button[] btnsBack;

        protected override async UniTask Initialize()
        {
            await base.Initialize();
            if (btnsBack != null)
            {
                foreach (var item in btnsBack)
                    item.onClick.AddListener(OnBack);
            }
        }

        protected virtual void OnBack()
        {
            ScreenContainer.Pop().Forget();
        }
    }
}
