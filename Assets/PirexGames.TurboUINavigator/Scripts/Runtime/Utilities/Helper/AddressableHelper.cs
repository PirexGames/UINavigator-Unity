using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
namespace PirexGames.Core.UINavigator
{
    public static class AddressableHelper<T>
    {
        private static Dictionary<string, AsyncOperationHandle<T>> cachedHandles = new Dictionary<string, AsyncOperationHandle<T>>();
        
        public static async UniTask<T> GetAsync(string key)
        {
            if (cachedHandles.TryGetValue(key, out var handle))
            {
                if (handle.IsValid()) return handle.Result;
                cachedHandles.Remove(key);
            }

            var asyncHandle = Addressables.LoadAssetAsync<T>(key);
            await asyncHandle.Task;
            
            if (asyncHandle.Status == AsyncOperationStatus.Succeeded)
            {
                cachedHandles[key] = asyncHandle;
                return asyncHandle.Result;
            }

            return default(T);
        }

        public static async UniTask<IList<T>> GetListAsync(string key)
        {
            var texTask = Addressables.LoadAssetsAsync<T>(key, tex => { });
            await texTask.Task;
            return texTask.Result;
        }

        public static void ClearCache()
        {
            if (cachedHandles == null) return;

            // Use a HashSet to ensure we only release each unique handle once
            // though with Dic<Key, Handle>, handles should be unique per key already.
            // Addressables handles are struct, so we check Validity.
            foreach (var handle in cachedHandles.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            cachedHandles.Clear();
        }
    }
}
