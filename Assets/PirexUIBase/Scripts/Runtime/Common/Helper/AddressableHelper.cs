using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
namespace Pirex.UI.Base
{
    public static class AddressableHelper<T>
    {
        private static Dictionary<string, T> cachedList = new Dictionary<string, T>();
        public static async UniTask<T> GetAsync(string key)
        {
            if (cachedList.TryGetValue(key, out var result))
            {
                return result;
            }
            var go = Addressables.LoadAssetAsync<T>(key);
            await go.Task;
            if (go.Status == AsyncOperationStatus.Succeeded)
            {
                cachedList[key] = go.Result;
                return go.Result;
            }
            return default(T);
        }

        public static async UniTask<IList<T>> GetListAsync(string key)
        {
            var texTask = Addressables.LoadAssetsAsync<T>(key, tex => { });
            await texTask.Task;
            return texTask.Result;
        }
    }
}
