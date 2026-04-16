using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
#if DOTWEEN_ENABLED
using DG.Tweening;
#endif

namespace PirexGames.Core.UINavigator
{
    public enum ModalStackMode { Single, Multiple }

    public abstract class BackdropStackContainer<T> : UIStackContainer<T> where T : ModalBaseView
    {
        [Header("Backdrop Settings")]
        [SerializeField] protected AnimationContainerEView backdropAnimation;
        [SerializeField] protected Color backdropColor = new Color(0, 0, 0, 0.5f);
        [SerializeField] protected ModalStackMode stackMode = ModalStackMode.Single;

        protected override async UniTask OnShowView(T view)
        {
            view.Owner = this;
            
            // Create per-modal backdrop
            var backdrop = CreateBackdrop(view);
            view.Backdrop = backdrop;

            // Sync transitions
            var tasks = ListPool<UniTask>.Get();
            tasks.Add(view.OnPushEnter());

            if (backdropAnimation != null && backdropAnimation.ShowAnimation != null)
                tasks.Add(backdropAnimation.ShowAnimation.Excute(backdrop, RectContainer));
            else
                tasks.Add(DefaultFade(backdrop, 1f));

            await AwaitTasks(tasks, true);
        }

        protected override async UniTask OnHideView(T view)
        {
            var backdrop = view.Backdrop;
            var tasks = ListPool<UniTask>.Get();
            tasks.Add(view.OnPopExit());

            if (backdrop != null)
            {
                if (backdropAnimation != null && backdropAnimation.HideAnimation != null)
                    tasks.Add(backdropAnimation.HideAnimation.Excute(backdrop, RectContainer));
                else
                    tasks.Add(DefaultFade(backdrop, 0f));

                try 
                {
                    await AwaitTasks(tasks, true);
                }
                finally
                {
                    if (backdrop != null && backdrop.gameObject != null)
                        Destroy(backdrop.gameObject);
                }
            }
            else
            {
                await AwaitTasks(tasks, true);
            }
        }

        protected override async UniTask OnBeforeHideTop(T currentTop, T nextTop)
        {
            if (stackMode == ModalStackMode.Multiple) return;

            var tasks = ListPool<UniTask>.Get();
            tasks.Add(currentTop.OnPushExit(nextTop));
            
            if (currentTop.Backdrop != null)
            {
                if (backdropAnimation != null && backdropAnimation.HideAnimation != null)
                    tasks.Add(backdropAnimation.HideAnimation.Excute(currentTop.Backdrop, RectContainer));
                else
                    tasks.Add(DefaultFade(currentTop.Backdrop, 0f));
            }

            await AwaitTasks(tasks, true);
        }

        protected override async UniTask OnBeforeShowNewTop(T newTop, T oldTop)
        {
            if (stackMode == ModalStackMode.Multiple) return;

            var tasks = ListPool<UniTask>.Get();
            tasks.Add(newTop.OnPopEnter());

            if (newTop.Backdrop != null)
            {
                if (backdropAnimation != null && backdropAnimation.ShowAnimation != null)
                    tasks.Add(backdropAnimation.ShowAnimation.Excute(newTop.Backdrop, RectContainer));
                else
                    tasks.Add(DefaultFade(newTop.Backdrop, 1f));
            }

            await AwaitTasks(tasks, true);
        }

        private CanvasGroup CreateBackdrop(T forView)
        {
            var overlayGO = new GameObject($"Backdrop_{forView.name}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            overlayGO.transform.SetParent(RectContainer, false);
            overlayGO.transform.SetSiblingIndex(forView.transform.GetSiblingIndex());

            var rect = overlayGO.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            var image = overlayGO.GetComponent<Image>();
            image.color = backdropColor;

            var cg = overlayGO.GetComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.blocksRaycasts = true;

            if (forView.CloseOnClickBackdrop)
            {
                var btn = overlayGO.AddComponent<Button>();
                btn.transition = Selectable.Transition.None;
                btn.onClick.AddListener(() => {
                    Pop(forView).Forget();
                });
            }

            return cg;
        }

        private async UniTask DefaultFade(CanvasGroup cg, float targetAlpha)
        {
            if (cg == null) return;
            float duration = 0.25f;

#if DOTWEEN_ENABLED
            await cg.DOFade(targetAlpha, duration).AsyncWaitForCompletion();
#else
            float startAlpha = cg.alpha;
            float elapsed = 0;

            while (elapsed < duration)
            {
                if (cg == null) break;
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                await UniTask.Yield();
            }

            if (cg != null) cg.alpha = targetAlpha;
#endif
            if (cg != null)
            {
                cg.blocksRaycasts = targetAlpha > 0;
            }
        }
    }
}
