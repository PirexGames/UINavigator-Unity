using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using DG.Tweening;
using TMPro;

namespace PirexGames.Core.UINavigator
{

    [CreateAssetMenu(fileName = "DataSceneTransition", menuName = "PirexGames.Core.UINavigator/Scene/DataSceneTransition", order = 0)]
    public class DataSceneTransition : ScriptableObject
    {
        private string keyAddressableScene;

        public void SetKeyAddressable(string key)
        {
            keyAddressableScene = key;
        }

        public string GetKeyAddressable()
            => keyAddressableScene;
    }
}
