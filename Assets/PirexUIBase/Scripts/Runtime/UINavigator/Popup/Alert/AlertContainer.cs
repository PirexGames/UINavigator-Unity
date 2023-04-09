using UnityEngine;
using System;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using DG.Tweening;

namespace Pirex.UI.Base
{
    public class AlertContainer : UIContainer
    {
        [SerializeField] protected GameObject modalBlockOverlayOverride;
        [SerializeField] protected float durationBlockOverlay = .15f;
        private static CanvasGroup modalBlockOverlay;
        private static float s_durationBlockOverlay = .15f;
        public static UIContainer Container;
        protected override void Awake()
        {
            base.Awake();
            Container = GetComponent<UIContainer>();
            GameObject modalOverlayPrefab = modalBlockOverlayOverride;
            if (!modalOverlayPrefab) return;
            GameObject modalSpawn;
            modalSpawn = Instantiate(modalOverlayPrefab, TransformContainer);
            modalBlockOverlay = modalSpawn.GetComponent<CanvasGroup>();
            modalBlockOverlay.alpha = 0;
            modalBlockOverlay.gameObject.SetActive(false);
        }

        public static async void Alert(string keyModal, string title, string content, string buttonNeutral)
        {
            var res = await SpawnAndInitModal(keyModal, title, content, null, null, buttonNeutral);
        }

        public static async void Alert(string keyModal, string title, string content, Action<CallbackStatus> callback, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            var res = await SpawnAndInitModal(keyModal, title, content, buttonPositive, buttonNegative, buttonNeutral);
            callback?.Invoke(res);
        }

        public static async void Alert(string keyModal, string title, string content, Action callback = null, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            var res = await SpawnAndInitModal(keyModal, title, content, buttonPositive, buttonNegative, buttonNeutral);
            if (res == CallbackStatus.Positive)
                callback?.Invoke();
        }


        private static async UniTask<CallbackStatus> SpawnAndInitModal(string keyModal, string title, string content, string buttonPositive, string buttonNegative, string buttonNeutral)
        {
            modalBlockOverlay.gameObject.SetActive(true);
            modalBlockOverlay.DOFade(1f, s_durationBlockOverlay);

            var modalSpawn = await Addressables.InstantiateAsync(keyModal, Container.TransformContainer).Task;
            var modal = modalSpawn.GetComponent<Alert>();
            if (modal is null) return CallbackStatus.None;
            var res = await modal.Init(title, content, buttonPositive, buttonNegative, buttonNeutral);
            Addressables.ReleaseInstance(modalSpawn);
            await modalBlockOverlay.DOFade(0f, s_durationBlockOverlay).AsyncWaitForCompletion();
            modalBlockOverlay.gameObject.SetActive(false);
            return res;
        }
    }

    public enum CallbackStatus
    {
        None,
        Positive,
        Negative,
        Neutral
    }
}