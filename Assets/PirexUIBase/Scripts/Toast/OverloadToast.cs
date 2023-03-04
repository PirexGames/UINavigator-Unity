using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pirex.UI.Base
{
    public partial class OverloadToast : MonoBehaviour
    {
        [SerializeField] private Transform externalParent;
        private static CancellationTokenSource _cancelToken;
        private static bool _isCurrentToastShowed = false;

        public async UniTask ShowToast(
            string keyAddressable,
            string content,
            float timeShow,
            Transform parent = null
        )
        {
            // if (_currentToast)
            // {
            //     if (!_isCurrentToastShowed) return;
            //     _cancelToken?.Cancel();
            //     Destroy(_currentToast.gameObject);
            // }
            //
            // _isCurrentToastShowed = false;
            // _cancelToken = new CancellationTokenSource();
            parent = externalParent;
            var toastPrefab = await AddressableHelperT<GameObject>.GetAsync(keyAddressable);
            var parentSpawn = parent ? parent : ScreenContainer.transformContainer;
            var toastSpawn = Instantiate(toastPrefab, parentSpawn);
            
            // Tính vị trí 
            
            var toast = toastSpawn.GetComponent<Toast>();
            // if (toast is null) return;
            toast.Init(content);
            await toast.OnShow();
            _isCurrentToastShowed = true;
            await UniTask.Delay((int) (timeShow * 1000));
            await toast.OnHide();
            Destroy(toast.gameObject);
        }
    }
}