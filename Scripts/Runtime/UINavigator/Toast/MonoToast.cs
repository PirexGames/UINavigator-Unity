using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.AddressableAssets;

namespace PirexGames.Core.UINavigator
{
    public class MonoToast : UIContainer
    {
        private static Toast _currentToast = null;
        private static CancellationTokenSource _cancelToken;
        private static bool _isCurrentToastShowed = false;
        public static UIContainer Container;

        protected override void Awake()
        {
            base.Awake();
            Container = GetComponent<MonoToast>();
        }

        public static async UniTask ShowToast(
            string keyAddressable,
            string content,
            float timeShow,
            Transform parent = null
        )
        {
            if (_currentToast)
            {
                if (!_isCurrentToastShowed) return;
                _cancelToken?.Cancel();
                await _currentToast.OnHide();
                Addressables.ReleaseInstance(_currentToast.gameObject);
                _currentToast = null;
            }
            _isCurrentToastShowed = false;
            _cancelToken = new CancellationTokenSource();
            var parentSpawn = parent ? parent : Container.TransformContainer;
            GameObject toastSpawn = await Addressables.InstantiateAsync(keyAddressable, parentSpawn).Task;

            var toast = toastSpawn.GetComponent<Toast>();
            if (toast is null) return;
            _currentToast = toast;
            _currentToast.Init(content);
            await _currentToast.OnShow();
            _isCurrentToastShowed = true;
            await UniTask.Delay((int)(timeShow * 1000), cancellationToken: _cancelToken.Token);
            await _currentToast.OnHide();

            if (_currentToast)
            {
                Addressables.ReleaseInstance(_currentToast.gameObject);
                _currentToast = null;
            }

        }
    }
}