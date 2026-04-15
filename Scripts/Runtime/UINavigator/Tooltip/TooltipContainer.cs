
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;

namespace PirexGames.Core.UINavigator
{
    public class TooltipContainer : UIContainer
    {
        public static UIContainer Container;
        private static Tooltip _currentTooltip = null;
        protected const string keyTooltipBlockOverlay = "TooltipBlockOverlay";
        private static CancellationTokenSource _cancelToken;
        protected override void Awake()
        {
            base.Awake();
            Container = GetComponent<TooltipContainer>();
            _currentTooltip = null;
        }

        public static async UniTask<Tooltip> Show(string keyAddressable, string content, RectTransform target, TooltipPosition tooltipPosition, bool isCloseAnyClick = false)
        {
            return await Show(keyAddressable, content, target, tooltipPosition, Vector2.zero, isCloseAnyClick);
        }

        public static async UniTask<Tooltip> Show(string keyAddressable, string content, RectTransform target, TooltipPosition tooltipPosition, Vector2 offset, bool isCloseAnyClick = false)
        {
            if (_currentTooltip != null)
            {
                CloseTooltipAsync();
            }

            var tooltipSpawn = await Addressables.InstantiateAsync(keyAddressable, Container.RectContainer).Task;
            var tooltip = tooltipSpawn.GetComponent<Tooltip>();
            _currentTooltip = tooltip;
            var tooltipRect = tooltipSpawn.GetComponent<RectTransform>();

            if (tooltip)
            {
                // Calculate target center in Container's local space
                Vector3 targetWorldCenter = target.TransformPoint(target.rect.center);
                Vector2 targetLocalCenter;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    Container.RectContainer,
                    RectTransformUtility.WorldToScreenPoint(null, targetWorldCenter),
                    null,
                    out targetLocalCenter
                );

                float hPos = 0f;
                float wPos = 0f;
                switch (tooltipPosition)
                {
                    case TooltipPosition.TOP:
                        hPos = target.rect.height / 2 + tooltipRect.rect.height / 2;
                        break;
                    case TooltipPosition.BOTTOM:
                        hPos = -target.rect.height / 2 - tooltipRect.rect.height / 2;
                        break;
                    case TooltipPosition.LEFT:
                        wPos = -target.rect.width / 2 - tooltipRect.rect.width / 2;
                        break;
                    case TooltipPosition.RIGHT:
                        wPos = target.rect.width / 2 + tooltipRect.rect.width / 2;
                        break;
                    case TooltipPosition.TOPLEFT:
                        hPos = target.rect.height / 2 + tooltipRect.rect.height / 2;
                        wPos = -target.rect.width / 2 - tooltipRect.rect.width / 2;
                        break;
                    case TooltipPosition.TOPRIGHT:
                        hPos = target.rect.height / 2 + tooltipRect.rect.height / 2;
                        wPos = target.rect.width / 2 + tooltipRect.rect.width / 2;
                        break;
                    case TooltipPosition.BOTTOMLEFT:
                        hPos = -target.rect.height / 2 - tooltipRect.rect.height / 2;
                        wPos = -target.rect.width / 2 - tooltipRect.rect.width / 2;
                        break;
                    case TooltipPosition.BOTTOMRIGHT:
                        hPos = -target.rect.height / 2 - tooltipRect.rect.height / 2;
                        wPos = target.rect.width / 2 + tooltipRect.rect.width / 2;
                        break;
                }

                Vector2 positionShow = targetLocalCenter + new Vector2(wPos, hPos) + offset;
                await tooltip.Init(content, positionShow, true);

                if (isCloseAnyClick)
                {
                    HandleAutoClose().Forget();
                }
            }

            return tooltip;
        }

        private static async UniTaskVoid HandleAutoClose()
        {
            // Wait a frame to avoid immediate closing if the click that opened it is still registered
            await UniTask.NextFrame();
            
            await UniTask.WaitUntil(() => 
                Pointer.current != null && 
                Pointer.current.press.wasPressedThisFrame
            );

            CloseTooltipAsync();
        }

        private static async void CloseTooltipAsync()
        {
            if (_currentTooltip == null) return;
            
            var tooltipToClose = _currentTooltip;
            _currentTooltip = null;

            await tooltipToClose.OnHide(tooltipToClose.transform.position);
            
            try
            {
                if (tooltipToClose.gameObject != null)
                {
                    Addressables.ReleaseInstance(tooltipToClose.gameObject);
                }
            }
            catch
            {
                // Ignore release errors if already destroyed
            }
        }
    }

    public enum TooltipPosition
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        TOPLEFT,
        TOPRIGHT,
        BOTTOMLEFT,
        BOTTOMRIGHT
    }
}
