using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressableHelper
{
    public static async UniTask<GameObject> GetModel(string key)
    {

        var go = Addressables.LoadAssetAsync<GameObject>(key);
        await go.Task;
        if (go.Status == AsyncOperationStatus.Succeeded)
        {
            return go.Result;
        }
        return null;
    }

    public static async UniTask<IList<Texture2D>> GetListTex2D(string key)
    {
        var texTask = Addressables.LoadAssetsAsync<Texture2D>(key, tex => { });
        await texTask.Task;
        return texTask.Result;
    }

    public static async UniTask<Texture2D> GetTexture2D(string key)
    {

        var tex = Addressables.LoadAssetAsync<Texture2D>(key);
        await tex.Task;
        if (tex.Status == AsyncOperationStatus.Succeeded)
        {
            return tex.Result;
        }
        return null;
    }

}