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
        public static UIContainer Container;
        private static float timeIntervalPush = 1f;
        protected static float currentTime;

        protected override void Awake()
        {
            base.Awake();
            currentTime = -timeIntervalPush * 2;
            Container = GetComponent<PurchaseModalContainer>();
            GameObject modalOverlayPrefab = modalBlockOverlayOverride;
            GameObject modalSpawn;
            modalSpawn = Instantiate(modalOverlayPrefab, TransformContainer);
            modalBlockOverlay = modalSpawn.GetComponent<CanvasGroup>();
            modalBlockOverlay.gameObject.SetActive(false);
        }

        public static async void Show(string keyModal, string id, string title,
            Sprite unitIcon, int pack, int cost, string nameUnit,
            string description,
            Sprite itemIcon,
            Action<int, string> callback,
            string buttonConfirm = null,
            int maxPack = 1
            )
        {

            await SpawnAndInitModal(keyModal, id, title, unitIcon, nameUnit, pack, cost, description, itemIcon, callback, buttonConfirm, maxPack);
        }

        private static async UniTask SpawnAndInitModal(string keyModal, string id, string title,
            Sprite unitIcon, string nameUnit, int pack, int cost,
            string description,
            Sprite itemIcon,
            Action<int, string> callback,
            string buttonConfirm,
            int maxPack = 1
            )
        {
            if (!ValidPush()) return;

            modalBlockOverlay?.gameObject.SetActive(true);
            modalBlockOverlay.DOFade(1f, s_durationBlockOverlay);

            var modalSpawn = await Addressables.InstantiateAsync(keyModal, Container.TransformContainer).Task;

            var modal = modalSpawn.GetComponent<PurchaseModal>();
            await modal.Init(id, title, description, itemIcon, pack, cost, unitIcon, nameUnit, buttonConfirm, callback, maxPack);
            await modalBlockOverlay.DOFade(0f, s_durationBlockOverlay).AsyncWaitForCompletion();
            modalBlockOverlay?.gameObject.SetActive(false);
            Addressables.ReleaseInstance(modalSpawn);
        }
        protected static bool ValidPush()
        {
            if (Time.fixedTime - currentTime >= timeIntervalPush)
            {
                currentTime = Time.fixedTime;
                return true;
            }
            return false;
        }
    }
}