using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Pirex.UI.Base
{
    public class ModalContainer : UIContainer
    {
        [SerializeField] protected GameObject modalBlockOverlay;
        [SerializeField] protected float durationBlockOverlay = .15f;
        private static CanvasGroup s_modalBlockOverlay;
        private static float s_durationBlockOverlay = .15f;
        protected const string keyModalBlockOverlay = "ModalBlockOverlay";
        public static UIContainer Container;
        protected override void Awake()
        {
            base.Awake();
            s_durationBlockOverlay = durationBlockOverlay;
            Container = GetComponent<ModalContainer>();
            GameObject modalOverlayPrefab = modalBlockOverlay;

            if (!modalOverlayPrefab) return;
            GameObject modalSpawn;
            modalSpawn = Instantiate(modalOverlayPrefab, TransformContainer);
            s_modalBlockOverlay = modalSpawn.GetComponent<CanvasGroup>();
            s_modalBlockOverlay.alpha = 0;
            s_modalBlockOverlay.gameObject.SetActive(false);
        }
        public async static UniTask Show(string keyModal, string title = null)
        {
            await SpawnAndInitModal(keyModal, title);
        }

        private static async UniTask SpawnAndInitModal(string keyModal, string title)
        {
            s_modalBlockOverlay.gameObject.SetActive(true);
            s_modalBlockOverlay.DOFade(1f, s_durationBlockOverlay);

            var modalSpawn = await Addressables.InstantiateAsync(keyModal, Container.TransformContainer).Task;

            var modal = modalSpawn.GetComponent<Modal>();
            if (modal is null) return;
            await modal.Init(title);
            Addressables.ReleaseInstance(modalSpawn);
            await s_modalBlockOverlay.DOFade(0f, s_durationBlockOverlay).AsyncWaitForCompletion();
            s_modalBlockOverlay.gameObject.SetActive(false);
        }
    }
}