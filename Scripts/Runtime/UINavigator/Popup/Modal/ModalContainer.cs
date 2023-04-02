using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Pirex.UI.Base
{
    public class ModalContainer : UIContainer
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
        public async static UniTask Show(string keyModal, string title = null)
        {
            modalBlockOverlay?.SetActive(true);
            await SpawnAndInitModal(keyModal, title);

        }

        private static async UniTask SpawnAndInitModal(string keyModal, string title)
        {
            var modalPrefab = await AddressableHelperT<GameObject>.GetAsync(keyModal);
            GameObject modalSpawn;
            if (Container.PoolSO)
                modalSpawn = Container.PoolSO.Spawn(modalPrefab, Container.TransformContainer);
            else
                modalSpawn = Instantiate(modalPrefab, Container.TransformContainer);

            var modal = modalSpawn.GetComponent<Modal>();
            if (modal is null) return;
            await modal.Init(title);
            if (Container.PoolSO)
                Container.PoolSO.DeSpawn(modalSpawn);
            else
                Destroy(modalSpawn);
            modalBlockOverlay?.SetActive(false);
        }
    }
}