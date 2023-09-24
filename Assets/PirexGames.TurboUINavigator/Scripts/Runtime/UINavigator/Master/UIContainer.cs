using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class UIContainer : MonoBehaviour
    {
        private Transform transformContainer;
        private RectTransform rectContainer;

        public Transform TransformContainer => transformContainer;
        public RectTransform RectContainer => rectContainer;

        protected virtual void Awake()
        {
            transformContainer = this.transform;
            rectContainer = (RectTransform)this.transform;
        }

        protected static async UniTask<GameObject> SpawnGO(string key, Transform parent)
        {
            var res = await SpawnGOHelper.Spawn(key, parent);
            return Instantiate(res, parent);
        }

    }

}