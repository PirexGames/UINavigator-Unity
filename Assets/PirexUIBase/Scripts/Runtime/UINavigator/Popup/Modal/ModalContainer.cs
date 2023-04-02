using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Pirex.UI.Base
{
    public class ModalContainer : UIContainer
    {
        [SerializeField] protected GameObject modalBlockOverlayOverride;
        [SerializeField] protected float durationBlockOverlay = .15f;
        private static CanvasGroup modalBlockOverlay;
        private static float s_durationBlockOverlay = .15f;
        protected const string keyModalBlockOverlay = "ModalBlockOverlay";
        public static UIContainer Container;
        protected async override void Awake()
        {
            base.Awake();
            s_durationBlockOverlay = durationBlockOverlay;
            Container = GetComponent<UIContainer>();
            GameObject modalOverlayPrefab;
            if (modalBlockOverlayOverride)
                modalOverlayPrefab = modalBlockOverlayOverride;
            else
                modalOverlayPrefab = await AddressableHelperT<GameObject>.GetAsync(keyModalBlockOverlay);
            if (!modalOverlayPrefab) return;
            GameObject modalSpawn;
            modalSpawn = Instantiate(modalOverlayPrefab, TransformContainer);
            modalBlockOverlay = modalSpawn.GetComponent<CanvasGroup>();
            modalBlockOverlay.alpha = 0;
            modalBlockOverlay.gameObject.SetActive(false);
        }
        public async static UniTask Show(string keyModal, string title = null)
        {
            modalBlockOverlay.gameObject.SetActive(true);
            await modalBlockOverlay.DOFade(1f, s_durationBlockOverlay).AsyncWaitForCompletion();
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
            await modalBlockOverlay.DOFade(0f, s_durationBlockOverlay).AsyncWaitForCompletion();
            modalBlockOverlay.gameObject.SetActive(false);
        }
    }
}