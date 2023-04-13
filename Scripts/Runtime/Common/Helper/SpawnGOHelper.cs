using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Pirex.UI.Base
{
    public static class SpawnGOHelper
    {
        public static async UniTask<GameObject> Spawn(string keyAddressable, Transform parent)
        {
            var res = await AddressableHelper<GameObject>.GetAsync(keyAddressable);
            return res;
        }
    }
}
