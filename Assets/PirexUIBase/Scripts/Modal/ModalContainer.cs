using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace Pirex.UI.Base
{
    public class ModalContainer : MonoBehaviour
    {
        [SerializeField] protected GameObject modalBlockOverlayOverride;
        public static Transform modalContainer;
        public static RectTransform rectModalContainer;
        private static GameObject modalBlockOverlay;
        protected const string keyModalBlockOverlay = "ModalBlockOverlay";
        private async void Awake()
        {
            modalContainer = GameObject.FindObjectOfType<ModalContainer>().transform;
            rectModalContainer = modalContainer.GetComponent<RectTransform>();
            GameObject modalOverlayPrefab;
            if (modalBlockOverlayOverride)
                modalOverlayPrefab = modalBlockOverlayOverride;
            else
                modalOverlayPrefab = await AddressableHelperT<GameObject>.GetAsync(keyModalBlockOverlay);
            var modalSpawn = Instantiate(modalOverlayPrefab, modalContainer);
            modalBlockOverlay = modalSpawn;
            modalBlockOverlay.SetActive(false);
        }
        public static void Alert(string keyModal, string title, GameObject content, Action<CallbackStatus> callback, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            modalBlockOverlay.SetActive(true);
            callback?.Invoke(CallbackStatus.Positive);
            modalBlockOverlay.SetActive(false);
        }

        public static async void Alert(string keyModal, string title, string content, Action<CallbackStatus> callback, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            modalBlockOverlay.SetActive(true);
            var res = await SpawnAndInitModal(keyModal, title, content, buttonPositive, buttonNegative, buttonNeutral);
            callback?.Invoke(res);
            modalBlockOverlay.SetActive(false);
        }

        public static void Alert(string keyModal, string title, GameObject content, Action callback = null, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            modalBlockOverlay.SetActive(true);
            callback?.Invoke();
            modalBlockOverlay.SetActive(false);
        }

        public static async void Alert(string keyModal, string title, string content, Action callback = null, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            modalBlockOverlay.SetActive(true);
            var res = await SpawnAndInitModal(keyModal, title, content, buttonPositive, buttonNegative, buttonNeutral);
            if (res == CallbackStatus.Positive)
                callback?.Invoke();
            modalBlockOverlay.SetActive(false);
        }

        public static async void Alert(string keyModal, string title, string content, string buttonPositive = null, string buttonNegative = null, string buttonNeutral = null)
        {
            modalBlockOverlay.SetActive(true);
            var res = await SpawnAndInitModal(keyModal, title, content, buttonPositive, buttonNegative, buttonNeutral);
            modalBlockOverlay.SetActive(false);
        }

        private static async UniTask<CallbackStatus> SpawnAndInitModal(string keyModal, string title, string content, string buttonPositive, string buttonNegative, string buttonNeutral)
        {
            var modalPrefab = await AddressableHelperT<GameObject>.GetAsync(keyModal);
            var modalSpawn = Instantiate(modalPrefab, modalContainer);

            var modal = modalSpawn.GetComponent<Modal>();
            if (modal is null) return CallbackStatus.None;
            var res = await modal.Init(title, content, buttonPositive, buttonNegative, buttonNeutral);
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