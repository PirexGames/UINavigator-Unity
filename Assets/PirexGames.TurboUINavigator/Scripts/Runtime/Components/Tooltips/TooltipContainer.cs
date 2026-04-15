using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;

namespace PirexGames.Core.UINavigator
{
    public class TooltipContainer : UIContainer<TooltipBaseView>
    {
        private TooltipBaseView _currentTooltipBase;

        public async UniTask<T> Show<T>(string key, string content, RectTransform target,
            TooltipPosition tooltipPosition, bool isCloseAnyClick = true) where T : TooltipBaseView
        {
            return await Show<T>(key, content, target, tooltipPosition, Vector2.zero, isCloseAnyClick);
        }
        public async UniTask<T> Show<T>(string key, string content, RectTransform target, TooltipPosition tooltipPosition, Vector2 offset, bool isCloseAnyClick = true) where T : TooltipBaseView
        {
            if (_currentTooltipBase != null)
            {
                await Hide();
            }

            var go = await SpawnView(key, RectContainer);
            var tooltip = go.GetComponent<T>();
            if (tooltip == null)
            {
                Debug.LogError($"[TooltipContainer] Prefab '{key}' does not have component '{typeof(T).Name}' attached!");
                return null;
            }

            _currentTooltipBase = tooltip;
            tooltip.NameAddressable = key;
            await tooltip.DoInitialize();

            // Calculate target center in Container's local space
            Vector3 targetWorldCenter = target.TransformPoint(target.rect.center);
            Vector2 targetLocalCenter;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                RectContainer,
                RectTransformUtility.WorldToScreenPoint(null, targetWorldCenter),
                null,
                out targetLocalCenter
            );

            var tooltipRect = (RectTransform)tooltip.transform;
            float pivotX = tooltipRect.pivot.x;
            float pivotY = tooltipRect.pivot.y;
            float tWidth = tooltipRect.rect.width;
            float tHeight = tooltipRect.rect.height;

            float hPos = 0f;
            float wPos = 0f;

            switch (tooltipPosition)
            {
                case TooltipPosition.Top:
                    hPos = (target.rect.height / 2) + (pivotY * tHeight);
                    break;
                case TooltipPosition.Bottom:
                    hPos = (-target.rect.height / 2) - ((1 - pivotY) * tHeight);
                    break;
                case TooltipPosition.Left:
                    wPos = (-target.rect.width / 2) - ((1 - pivotX) * tWidth);
                    break;
                case TooltipPosition.Right:
                    wPos = (target.rect.width / 2) + (pivotX * tWidth);
                    break;
                case TooltipPosition.TopLeft:
                    hPos = (target.rect.height / 2) + (pivotY * tHeight);
                    wPos = (-target.rect.width / 2) - ((1 - pivotX) * tWidth);
                    break;
                case TooltipPosition.TopRight:
                    hPos = (target.rect.height / 2) + (pivotY * tHeight);
                    wPos = (target.rect.width / 2) + (pivotX * tWidth);
                    break;
                case TooltipPosition.BottomLeft:
                    hPos = (-target.rect.height / 2) - ((1 - pivotY) * tHeight);
                    wPos = (-target.rect.width / 2) - ((1 - pivotX) * tWidth);
                    break;
                case TooltipPosition.BottomRight:
                    hPos = (-target.rect.height / 2) - ((1 - pivotY) * tHeight);
                    wPos = (target.rect.width / 2) + (pivotX * tWidth);
                    break;
            }

            Vector2 positionShow = targetLocalCenter + new Vector2(wPos, hPos) + offset;

            if (tooltipPosition == TooltipPosition.Auto && tooltip is TooltipBaseAutoView autoTooltip)
            {
                await autoTooltip.InitAuto(content, target, RectContainer, offset);
            }
            else
            {
                await tooltip.Init(content, positionShow, true);
            }

            if (isCloseAnyClick)
            {
                HandleAutoClose(tooltip).Forget();
            }

            return tooltip;
        }

        private async UniTaskVoid HandleAutoClose(TooltipBaseView tooltipBase)
        {
            await UniTask.NextFrame();
            await UniTask.WaitUntil(() => 
                Pointer.current != null && Pointer.current.press.wasPressedThisFrame,
                cancellationToken: tooltipBase.SessionToken
            );

            if (tooltipBase == _currentTooltipBase)
            {
                await Hide();
            }
        }

        public async UniTask Hide()
        {
            if (_currentTooltipBase == null) return;

            var tooltip = _currentTooltipBase;
            _currentTooltipBase = null; // Prevent re-entry immediately

            await tooltip.OnHide();
            
            if (tooltip && tooltip.gameObject)
            {
                if (tooltip.IsPooling)
                {
                    UIContainer.ReturnToPool(tooltip.NameAddressable, tooltip.gameObject);
                }
                else
                {
                    Destroy(tooltip.gameObject);
                }
            }
        }
    }

    [Flags]
    public enum TooltipPosition
    {
        None = 0,
        Top = 1 << 0,
        Bottom = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        TopLeft = 1 << 4,
        TopRight = 1 << 5,
        BottomLeft = 1 << 6,
        BottomRight = 1 << 7,
        Auto = 1 << 8
    }
}
