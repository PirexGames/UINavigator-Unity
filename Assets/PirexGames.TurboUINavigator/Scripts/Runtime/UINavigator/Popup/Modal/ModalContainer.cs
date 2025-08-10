using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PirexGames.Core.UINavigator
{
    public class ModalContainer : UIContainer
    {
        [SerializeField] protected GameObject modalBlockOverlay;
        [SerializeField] protected float durationBlockOverlay = .15f;

        private static Stack<Modal> stackModal = new();
        private static CanvasGroup s_modalBlockOverlay;
        private static float s_durationBlockOverlay = .15f;
        protected const string keyModalBlockOverlay = "ModalBlockOverlay";
        internal static UIContainer Container;

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
        public static async UniTask<Modal> Show(string keyModal, string title = null)
        {
            return await SpawnAndInitModal(keyModal, title);
        }

        private static async UniTask<Modal> SpawnAndInitModal(string keyModal, string title)
        {
            s_modalBlockOverlay.gameObject.SetActive(true);
            s_modalBlockOverlay.DOFade(1f, s_durationBlockOverlay);

            var modalSpawn = await Addressables.InstantiateAsync(keyModal, Container.TransformContainer).Task;

            var modal = modalSpawn.GetComponent<Modal>();
            modal.Init(title).Forget();
            stackModal.Push(modal);
            return modal;
        }

        public static async UniTask Pop()
        {
            if (stackModal.TryPop(out var modal))
            {
                await modal.OnHide();
                Addressables.ReleaseInstance(modal.gameObject);
 
            }

            if (stackModal.Count <= 0)
            {
                await s_modalBlockOverlay.DOFade(0f, s_durationBlockOverlay).AsyncWaitForCompletion();
                s_modalBlockOverlay.gameObject.SetActive(false);
            }
        }
    }
}