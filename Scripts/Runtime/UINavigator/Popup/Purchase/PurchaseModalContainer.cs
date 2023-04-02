using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pirex.UI.Base;
using UnityEngine;
namespace Pirex.UI.Base
{
    public class PurchaseModalContainer : UIContainer
    {
        [SerializeField] protected GameObject modalBlockOverlayOverride;
        private static GameObject modalBlockOverlay;
        protected const string keyModalBlockOverlay = "ModalBlockOverlay";
        public static UIContainer Container;

        protected async override void Awake()
        {
            base.Awake();
            Container = GetComponent<UIContainer>();
            GameObject modalOverlayPrefab;
            if (modalBlockOverlayOverride)
                modalOverlayPrefab = modalBlockOverlayOverride;
            else
                modalOverlayPrefab = await AddressableHelperT<GameObject>.GetAsync(keyModalBlockOverlay);
            if (!modalOverlayPrefab) return;
            GameObject modalSpawn;
            modalSpawn = Instantiate(modalOverlayPrefab, TransformContainer);
            modalBlockOverlay = modalSpawn;
            modalBlockOverlay.SetActive(false);
        }

        public static async void Show(string keyModal, string id, string title, Texture2D unit, int pack, int cost,
            string description,
            Texture2D texture2D,
            Action<int, string> callback,
            string buttonConfirm = null)
        {
            modalBlockOverlay?.SetActive(true);
            await SpawnAndInitModal(keyModal, id, title, unit, pack, cost, description, texture2D, callback, buttonConfirm);
        }

        private static async UniTask SpawnAndInitModal(string keyModal, string id, string title, Texture2D unit,
            int pack, int cost,
            string description,
            Texture2D texture2D,
            Action<int, string> callback,
            string buttonConfirm)
        {
            var modalPrefab = await AddressableHelperT<GameObject>.GetAsync(keyModal);
            GameObject modalSpawn;
            if (Container.PoolSO)
                modalSpawn = Container.PoolSO.Spawn(modalPrefab, Container.TransformContainer);
            else
                modalSpawn = Instantiate(modalPrefab, Container.TransformContainer);

            var modal = modalSpawn.GetComponent<PurchaseModal>();
            if (modal is null) return;
            var res = await modal.Init(title, description, texture2D, buttonConfirm);
            modalBlockOverlay?.SetActive(false);
            if (res.Equals(1)) callback?.Invoke(res, id);
            if (Container.PoolSO)
                Container.PoolSO.DeSpawn(modalSpawn);
            else
                Destroy(modalSpawn);
        }
    }
}