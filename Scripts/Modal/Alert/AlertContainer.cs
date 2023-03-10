using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace Pirex.UI.Base
{
    public class AlertContainer : UIContainer
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
        public static void Alert(string keyModal, string title, GameObject content, Action<CallbackStatus> callback, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            modalBlockOverlay?.SetActive(true);
            callback?.Invoke(CallbackStatus.Positive);
        }

        public static async void Alert(string keyModal, string title, string content, Action<CallbackStatus> callback, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            modalBlockOverlay?.SetActive(true);
            var res = await SpawnAndInitModal(keyModal, title, content, buttonPositive, buttonNegative, buttonNeutral);
            callback?.Invoke(res);
        }

        public static void Alert(string keyModal, string title, GameObject content, Action callback = null, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            modalBlockOverlay?.SetActive(true);
            callback?.Invoke();
        }

        public static async void Alert(string keyModal, string title, string content, Action callback = null, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            modalBlockOverlay?.SetActive(true);
            var res = await SpawnAndInitModal(keyModal, title, content, buttonPositive, buttonNegative, buttonNeutral);
            if (res == CallbackStatus.Positive)
                callback?.Invoke();
        }

        public static async void Alert(string keyModal, string title, string content, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            modalBlockOverlay?.SetActive(true);
            var res = await SpawnAndInitModal(keyModal, title, content, buttonPositive, buttonNegative, buttonNeutral);
        }

        private static async UniTask<CallbackStatus> SpawnAndInitModal(string keyModal, string title, string content, string buttonPositive, string buttonNegative, string buttonNeutral)
        {
            var modalPrefab = await AddressableHelperT<GameObject>.GetAsync(keyModal);
            GameObject modalSpawn;
            if (Container.PoolSO)
                modalSpawn = Container.PoolSO.Spawn(modalPrefab, Container.TransformContainer);
            else
                modalSpawn = Instantiate(modalPrefab, Container.TransformContainer);

            var modal = modalSpawn.GetComponent<Alert>();
            if (modal is null) return CallbackStatus.None;
            var res = await modal.Init(title, content, buttonPositive, buttonNegative, buttonNeutral);
            if (Container.PoolSO)
                Container.PoolSO.DeSpawn(modalSpawn);
            else
                Destroy(modalSpawn);
            modalBlockOverlay?.SetActive(false);
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