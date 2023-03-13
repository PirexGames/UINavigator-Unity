using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Pirex.UI.Base
{
    public partial class MonoToast : UIContainer
    {
        private static Toast _currentToast = null;
        private static CancellationTokenSource _cancelToken;
        private static bool _isCurrentToastShowed = false;
        public static UIContainer Container;

        protected override void Awake()
        {
            base.Awake();
            Container = GetComponent<UIContainer>();
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
                if (Container.PoolSO)
                {
                    Container.PoolSO.DeSpawn(_currentToast.gameObject);
                    _currentToast = null;
                    await UniTask.NextFrame();
                }
                else
                    Destroy(_currentToast.gameObject);
            }
            _isCurrentToastShowed = false;
            _cancelToken = new CancellationTokenSource();
            var toastPrefab = await AddressableHelperT<GameObject>.GetAsync(keyAddressable);
            var parentSpawn = parent ? parent : ScreenContainer.Container.TransformContainer;
            GameObject toastSpawn;
            if (Container.PoolSO)
                toastSpawn = Container.PoolSO.Spawn(toastPrefab, parentSpawn);
            else toastSpawn = Instantiate(toastPrefab, parentSpawn);

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
                if (Container.PoolSO)
                {
                    Container.PoolSO.DeSpawn(_currentToast.gameObject);
                    _currentToast = null;
                }
                else
                    Destroy(_currentToast.gameObject);
            }

        }
    }
}