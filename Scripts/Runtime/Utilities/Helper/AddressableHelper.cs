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
    public static class AddressableHelper
    {
        private static List<Action> _clearActions = new List<Action>();
        
        public static void RegisterClearAction(Action action)
        {
            if (!_clearActions.Contains(action))
                _clearActions.Add(action);
        }

        public static void ClearAllCaches()
        {
            foreach (var action in _clearActions)
            {
                action?.Invoke();
            }
        }
    }

    public static class AddressableHelper<T>
    {
        static AddressableHelper()
        {
            AddressableHelper.RegisterClearAction(ClearCache);
        }

        private static Dictionary<string, AsyncOperationHandle<T>> cachedHandles = new Dictionary<string, AsyncOperationHandle<T>>();
        private static Dictionary<string, AsyncOperationHandle<IList<T>>> cachedListHandles = new Dictionary<string, AsyncOperationHandle<IList<T>>>();
        
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
            if (cachedListHandles.TryGetValue(key, out var handle))
            {
                if (handle.IsValid()) return handle.Result;
                cachedListHandles.Remove(key);
            }

            var texTask = Addressables.LoadAssetsAsync<T>(key, null);
            await texTask.Task;
            
            if (texTask.Status == AsyncOperationStatus.Succeeded)
            {
                cachedListHandles[key] = texTask;
                return texTask.Result;
            }

            return null;
        }

        public static void ClearCache()
        {
            if (cachedHandles != null)
            {
                foreach (var handle in cachedHandles.Values)
                {
                    if (handle.IsValid()) Addressables.Release(handle);
                }
                cachedHandles.Clear();
            }

            if (cachedListHandles != null)
            {
                foreach (var handle in cachedListHandles.Values)
                {
                    if (handle.IsValid()) Addressables.Release(handle);
                }
                cachedListHandles.Clear();
            }
        }
    }
}
