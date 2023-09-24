using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class ModalCustomize : Modal
    {
        bool isSetAsync = false;
        protected async override UniTask Initialize()
        {
            await UniTask.WaitUntil(() => isSetAsync);
            await base.Initialize();
        }

        public async void SetUpAsync(string ahihi)
        {
            await UniTask.Delay(2000);
            isSetAsync = true;
        }
    }
}