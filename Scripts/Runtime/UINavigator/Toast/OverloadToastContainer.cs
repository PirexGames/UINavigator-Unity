using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PirexGames.Core.UINavigator
{
    public class OverloadToastContainer : UIContainer
    {
        [HideInInspector] public UIContainer Container;

        protected override void Awake()
        {
            base.Awake();
            Container = GetComponent<OverloadToastContainer>();
        }

        public async UniTask ShowToast(
            string keyAddressable,
            string content,
            float timeShow,
            Transform parent = null
        )
        {
            parent = Container.TransformContainer;
            var parentSpawn = parent ? parent : ScreenContainer.Container.TransformContainer;

            GameObject toastSpawn = await Addressables.InstantiateAsync(keyAddressable, parentSpawn).Task;

            var toast = toastSpawn.GetComponent<Toast>();
            if (toast is null) return;
            toast.Init(content);
            await toast.OnShow();
            await UniTask.Delay((int)(timeShow * 1000));
            await toast.OnHide();

            Addressables.ReleaseInstance(toast.gameObject);
        }
    }
}