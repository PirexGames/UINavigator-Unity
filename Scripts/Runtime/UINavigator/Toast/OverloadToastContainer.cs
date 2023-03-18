using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pirex.UI.Base
{
    public partial class OverloadToastContainer : UIContainer
    {
        [HideInInspector] public UIContainer Container;

        protected override void Awake()
        {
            base.Awake();
            Container = GetComponent<UIContainer>();
        }

        public async UniTask ShowToast(
            string keyAddressable,
            string content,
            float timeShow,
            Transform parent = null
        )
        {

            parent = Container.TransformContainer;
            var toastPrefab = await AddressableHelperT<GameObject>.GetAsync(keyAddressable);
            var parentSpawn = parent ? parent : ScreenContainer.Container.TransformContainer;

            GameObject toastSpawn;

            if (Container.PoolSO)
                toastSpawn = Container.PoolSO.Spawn(toastPrefab, parentSpawn);
            else
                toastSpawn = Instantiate(toastPrefab, parentSpawn);

            // Tính vị trí 

            var toast = toastSpawn.GetComponent<Toast>();
            // if (toast is null) return;
            toast.Init(content);
            await toast.OnPushEnter();
            await UniTask.Delay((int)(timeShow * 1000));
            await toast.OnPopExit();

            if (Container.PoolSO)
                Container.PoolSO.DeSpawn(toast.gameObject);
            else
                Destroy(toast.gameObject);
        }
    }
}