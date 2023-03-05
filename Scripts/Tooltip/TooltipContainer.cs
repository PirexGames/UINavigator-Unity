
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pirex.UI.Base
{
    public class TooltipContainer : UIContainer
    {
        public static UIContainer Container;
        private static Tooltip _currentTooltip = null;
        private static Button tooltipBlock;
        protected const string keyTooltipBlockOverlay = "TooltipBlockOverlay";
        private static CancellationTokenSource _cancelToken;
        protected async override void Awake()
        {
            base.Awake();
            Container = GetComponent<UIContainer>();
            GameObject modalOverlayPrefab;
            modalOverlayPrefab = await AddressableHelperT<GameObject>.GetAsync(keyTooltipBlockOverlay);
            if (!modalOverlayPrefab) return;
            GameObject tooltipBlockSpawn;
            tooltipBlockSpawn = Instantiate(modalOverlayPrefab, TransformContainer);
            tooltipBlock = tooltipBlockSpawn.GetComponent<Button>();
            tooltipBlock.onClick.AddListener(CloseTooltipAsync);
            tooltipBlock.gameObject.SetActive(false);
        }
        public static async UniTask Show(string keyAddressable, string content, RectTransform parent, TooltipPosition tooltipPosition, bool isCloseAnyClick = false)
        {
            await Show(keyAddressable, content, parent, tooltipPosition, Vector2.zero, isCloseAnyClick);
            await UniTask.CompletedTask;
        }

        public static async UniTask Show(string keyAddressable, string content, RectTransform parent, TooltipPosition tooltipPosition, Vector2 offset, bool isCloseAnyClick = false)
        {
            tooltipBlock.gameObject.SetActive(true);
            if (_currentTooltip)
            {
                _cancelToken?.Cancel();
                await _currentTooltip.OnHide();
                if (Container.PoolSO)
                    Container.PoolSO.DeSpawn(_currentTooltip.gameObject);
                else
                    Destroy(_currentTooltip.gameObject);
                _currentTooltip = null;
            }

            _cancelToken = new CancellationTokenSource();
            var tooltipPrefab = await AddressableHelperT<GameObject>.GetAsync(keyAddressable);
            GameObject tooltipSpawn;
            if (Container.PoolSO)
                tooltipSpawn = Container.PoolSO.Spawn(tooltipPrefab, parent);
            else
                tooltipSpawn = Instantiate(tooltipPrefab, parent);
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
            await UniTask.WaitUntil(() => _currentTooltip == null, cancellationToken: _cancelToken.Token);
            await UniTask.CompletedTask;
        }

        private async void CloseTooltipAsync()
        {
            await _currentTooltip.OnHide();
            if (Container.PoolSO)
                Container.PoolSO.DeSpawn(_currentTooltip.gameObject);
            else
                Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
            tooltipBlock.gameObject.SetActive(false);
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
