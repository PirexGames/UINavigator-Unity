using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class AddressableHelperT<T>
{
    public static async UniTask<T> GetAsync(string key)
    {

        var go = Addressables.LoadAssetAsync<T>(key);
        await go.Task;
        if (go.Status == AsyncOperationStatus.Succeeded)
        {
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