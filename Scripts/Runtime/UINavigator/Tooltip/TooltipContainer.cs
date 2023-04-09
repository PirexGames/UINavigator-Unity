
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;

namespace Pirex.UI.Base
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
            Container = GetComponent<OverloadToastContainer>();
            _currentTooltip = null;
        }

        public static async UniTask Show(string keyAddressable, string content, RectTransform parent, TooltipPosition tooltipPosition, bool isCloseAnyClick = false)
        {
            await Show(keyAddressable, content, parent, tooltipPosition, Vector2.zero, isCloseAnyClick);
            await UniTask.CompletedTask;
        }

        public static async UniTask Show(string keyAddressable, string content, RectTransform parent, TooltipPosition tooltipPosition, Vector2 offset, bool isCloseAnyClick = false)
        {
            await UniTask.WaitUntil(() => _currentTooltip == null);
            var tooltipSpawn = await Addressables.InstantiateAsync(keyAddressable, parent).Task;

            var tooltip = tooltipSpawn.GetComponent<Tooltip>();
            _currentTooltip = tooltip;
            var tooltipRect = tooltipSpawn.GetComponent<RectTransform>();
            if (tooltip)
            {
                Vector2 positionShow;
                float hPos = 0f;
                float wPos = 0f;
                switch (tooltipPosition)
                {
                    case TooltipPosition.TOP:
                        hPos = parent.rect.height / 2 + tooltipRect.rect.height / 2;
                        break;
                    case TooltipPosition.BOTTOM:
                        hPos = -parent.rect.height / 2 - tooltipRect.rect.height / 2;
                        break;
                    case TooltipPosition.LEFT:
                        wPos = -parent.rect.width / 2 - tooltipRect.rect.width / 2;
                        break;
                    case TooltipPosition.RIGHT:
                        wPos = parent.rect.width / 2 + tooltipRect.rect.width / 2;
                        break;
                    case TooltipPosition.TOPLEFT:
                        hPos = parent.rect.height / 2 + tooltipRect.rect.height / 2;
                        wPos = -parent.rect.width / 2 - tooltipRect.rect.width / 2;
                        break;
                    case TooltipPosition.TOPRIGHT:
                        hPos = parent.rect.height / 2 + tooltipRect.rect.height / 2;
                        wPos = parent.rect.width / 2 + tooltipRect.rect.width / 2;
                        break;
                    case TooltipPosition.BOTTOMLEFT:
                        hPos = -parent.rect.height / 2 - tooltipRect.rect.height / 2;
                        wPos = -parent.rect.width / 2 - tooltipRect.rect.width / 2;
                        break;
                    case TooltipPosition.BOTTOMRIGHT:
                        hPos = -parent.rect.height / 2 - tooltipRect.rect.height / 2;
                        wPos = parent.rect.width / 2 + tooltipRect.rect.width / 2;
                        break;
                }
                positionShow = new Vector2(wPos, hPos) + offset;
                tooltip.Init(content, positionShow, true);
            }
            var mousePressStream = InputSystem.onEvent
                .Where(_ => Input.GetMouseButtonDown(0))
                .Call(_ => CloseTooltipAsync());

            await UniTask.WaitUntil(() => _currentTooltip == null);
            mousePressStream.Dispose();
            await UniTask.CompletedTask;
        }

        private async static void CloseTooltipAsync()
        {
            if (!_currentTooltip)
                if (!_currentTooltip.gameObject) return;
            await _currentTooltip.OnHide(_currentTooltip.transform.position);
            try
            {
                Addressables.ReleaseInstance(_currentTooltip.gameObject);
            }
            catch
            {

            }
            _currentTooltip = null;
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
