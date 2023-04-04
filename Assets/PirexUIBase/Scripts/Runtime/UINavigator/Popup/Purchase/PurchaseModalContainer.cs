using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pirex.UI.Base;
using UnityEngine;
using UnityEngine.AddressableAssets;
using DG.Tweening;

namespace Pirex.UI.Base
{
    public class PurchaseModalContainer : UIContainer
    {
        [SerializeField] protected GameObject modalBlockOverlayOverride;
        private static float s_durationBlockOverlay = .15f;
        private static CanvasGroup modalBlockOverlay;
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
            modalBlockOverlay = modalSpawn.GetComponent<CanvasGroup>();
            modalBlockOverlay.gameObject.SetActive(false);
        }

        public static async void Show(string keyModal, string id, string title, Sprite unitIcon, int pack, int cost,
            string description,
            Sprite itemIcon,
            Action<int, string> callback,
            string buttonConfirm = null)
        {
            modalBlockOverlay?.gameObject.SetActive(true);
            await modalBlockOverlay.DOFade(1f, s_durationBlockOverlay).AsyncWaitForCompletion();
            await SpawnAndInitModal(keyModal, id, title, unitIcon, pack, cost, description, itemIcon, callback, buttonConfirm);
        }

        private static async UniTask SpawnAndInitModal(string keyModal, string id, string title, Sprite unitIcon,
            int pack, int cost,
            string description,
            Sprite itemIcon,
            Action<int, string> callback,
            string buttonConfirm)
        {
            var modalSpawn = await Addressables.InstantiateAsync(keyModal, Container.TransformContainer).Task;

            var modal = modalSpawn.GetComponent<PurchaseModal>();
            if (modal is null) return;
            var res = await modal.Init(title, description, itemIcon, buttonConfirm);
            await modalBlockOverlay.DOFade(0f, s_durationBlockOverlay).AsyncWaitForCompletion();
            modalBlockOverlay?.gameObject.SetActive(false);
            if (res.Equals(1)) callback?.Invoke(res, id);
            Addressables.ReleaseInstance(modalSpawn);
        }
    }
}