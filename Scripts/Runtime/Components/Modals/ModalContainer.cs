using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class ModalContainer : BackdropStackContainer<ModalBaseView>
    {
        // Shorthand for simple push with closeOnClickBackdrop override
        public virtual async UniTask<T> Push<T>(string key, bool closeOnClickBackdrop, ViewPushMode mode = ViewPushMode.Standard, bool awaitAnimation = false, bool enableBackdrop = true) where T : ModalBaseView
        {
            return await base.Push<T>(key, mode, awaitAnimation, (modal) =>
            {
                modal.CloseOnClickBackdrop = closeOnClickBackdrop;
                modal.EnableBackdrop = enableBackdrop;
            });
        }
    }
}
