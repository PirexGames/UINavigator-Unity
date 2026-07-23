using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if LITMOTION_ENABLED
using LitMotion;
using LitMotion.Extensions;
#elif DOTWEEN_ENABLED
using DG.Tweening;
#endif
using Cysharp.Threading.Tasks;

namespace PirexGames.Core.UINavigator
{
    public class DynamicSheetContainer : MonoBehaviour
    {
        [SerializeField] private bool isAutoOperation = true;
        [SerializeField] private bool activeSelfSheet = true;
        [SerializeField] private RectTransform canvas;
        [SerializeField] private bool allowMultiActive;
        [SerializeField] private bool allowSwitchOff;
        [SerializeField] private List<SheetContainerType> sheets = new List<SheetContainerType>();
        [SerializeField] private float duration;
        [SerializeField] private ScreenPosition sheetPosition;
        [SerializeField] private CanvasGroup overlay;
        private SheetContainerType activeSheet;

        private Dictionary<Toggle, SheetContainerType> _toggleCache;
        private Dictionary<RectTransform, SheetContainerType> _rectCache;

        private void Start()
        {
            int count = sheets.Count;
            _toggleCache = new Dictionary<Toggle, SheetContainerType>(count);
            _rectCache = new Dictionary<RectTransform, SheetContainerType>(count);

            for (int i = 0; i < count; i++)
            {
                var item = sheets[i];
                if (item.toggle != null) _toggleCache[item.toggle] = item;
                if (item.rectTransform != null) _rectCache[item.rectTransform] = item;

                if (isAutoOperation && item.toggle != null)
                {
                    item.toggle.onValueChanged.AddListener(_ => TransitionOn(item.toggle));
                }
                
                if (item.cancelButton != null)
                    item.cancelButton.onClick.AddListener(TransitionOff);
            }

            if (!allowSwitchOff && sheets.Count > 0)
            {
                TransitionOn(sheets[0].toggle);
            }
        }

        public void TransitionOn(RectTransform rectTransform)
        {
            SheetContainerType sheet = FindSheet(rectTransform);
            if (sheet != null && sheet.toggle != null && sheet.toggle.isOn)
            {
                TransitionSheetAsync(sheet, sheet.toggle.isOn).Forget();
                return;
            }
            TransitionSheet(sheet);
        }

        public void TransitionOn(Toggle toggle)
        {
            SheetContainerType sheet = FindSheet(toggle);
            if (sheet == null)
            {
                if (allowSwitchOff) TransitionOff();
                return;
            }
            if (!toggle.isOn)
            {
                TransitionSheetAsync(sheet, toggle.isOn).Forget();
                return;
            }
            TransitionSheet(sheet);
        }

        public void TransitionOnAll()
        {
            if (!allowMultiActive) return;
            for (int i = 0; i < sheets.Count; i++)
            {
                sheets[i].toggle.isOn = true;
            }
        }

        public void TransitionSheet(SheetContainerType sheet)
        {
            if (sheet == activeSheet)
            {
                if (!allowSwitchOff) return;
            };
            if (sheet == null) return;
            if (activeSheet != null && !allowMultiActive)
                if (activeSheet.rectTransform != null && activeSheet.canvasGroup != null && activeSheet.toggle != null)
                    TransitionSheetAsync(activeSheet, false).Forget();
            activeSheet = sheet;
            TransitionSheetAsync(activeSheet, sheet.toggle.isOn).Forget();
        }

        public void TransitionOff()
        {
            if (activeSelfSheet && activeSheet != null && activeSheet.toggle != null)
                activeSheet.toggle.isOn = false;
        }

        public void TransitionOffAll()
        {
            if (!allowMultiActive) return;
            for (int i = 0; i < sheets.Count; i++)
            {
                sheets[i].toggle.isOn = false;
            }
        }

        private SheetContainerType FindSheet(Toggle toggle)
        {
            if (toggle == null) return null;
            if (_toggleCache != null && _toggleCache.TryGetValue(toggle, out var sheet))
                return sheet;
            
            // Fallback
            for (int i = 0; i < sheets.Count; i++)
            {
                if (sheets[i].toggle == toggle) return sheets[i];
            }
            return null;
        }

        private SheetContainerType FindSheet(RectTransform rect)
        {
            if (rect == null) return null;
            if (_rectCache != null && _rectCache.TryGetValue(rect, out var sheet))
                return sheet;

            // Fallback
            for (int i = 0; i < sheets.Count; i++)
            {
                if (sheets[i].rectTransform == rect) return sheets[i];
            }
            return null;
        }

        public async UniTask TransitionSheetAsync(SheetContainerType sheet, bool isOn)
        {
            if (sheet == null || sheet.rectTransform == null || sheet.canvasGroup == null) return;

            var sheetRect = sheet.rectTransform;
            float xPivot = sheetRect.pivot.x;
            float yPivot = sheetRect.pivot.y;

            float xPivotMinus05 = xPivot - 0.5f;
            float yPivotMinus05 = yPivot - 0.5f;

            float xPivot1Minus = 1 - xPivot;
            float yPivot1Minus = 1 - yPivot;

            float width = sheetRect.rect.width;
            float height = sheetRect.rect.height;

            float wCanvas = canvas != null ? canvas.rect.width : 0;
            float hCanvas = canvas != null ? canvas.rect.height : 0;

            float hPosMoveTo = 0;
            float wPosMoveTo = 0;

            var position = sheetPosition != ScreenPosition.NONE ? sheetPosition : sheet.position;

            switch (position)
            {
                case ScreenPosition.TOP:
                    wPosMoveTo += width * (sheet.isPivotEffectPosition ? 0 : xPivotMinus05);
                    if (isOn)
                        hPosMoveTo = hCanvas / 2 - height * (sheet.isPivotEffectPosition ? 0 : yPivot1Minus);
                    else
                        hPosMoveTo = hCanvas / 2 + height * (sheet.isPivotEffectPosition ? 1 : yPivot);
                    break;
                case ScreenPosition.BOTTOM:
                    wPosMoveTo += width * (sheet.isPivotEffectPosition ? 0 : xPivotMinus05);
                    if (isOn)
                        hPosMoveTo = -hCanvas / 2 + height * (sheet.isPivotEffectPosition ? 0 : yPivot);
                    else
                        hPosMoveTo = -hCanvas / 2 - height * (sheet.isPivotEffectPosition ? 1 : yPivot1Minus);
                    break;
                case ScreenPosition.LEFT:
                    hPosMoveTo += height * (sheet.isPivotEffectPosition ? 0 : yPivotMinus05);
                    if (isOn)
                        wPosMoveTo = -wCanvas / 2 + width * (sheet.isPivotEffectPosition ? 0 : xPivot);
                    else
                        wPosMoveTo = -wCanvas / 2 - width * (sheet.isPivotEffectPosition ? 1 : xPivot1Minus);
                    break;
                case ScreenPosition.RIGHT:
                    hPosMoveTo += height * (sheet.isPivotEffectPosition ? 0 : yPivotMinus05);
                    if (isOn)
                        wPosMoveTo = wCanvas / 2 - width * (sheet.isPivotEffectPosition ? 0 : xPivot1Minus);
                    else
                        wPosMoveTo = wCanvas / 2 + width * (sheet.isPivotEffectPosition ? 1 : xPivot);
                    break;

                case ScreenPosition.TOPLEFT:
                    hPosMoveTo = hCanvas / 2 - height * (sheet.isPivotEffectPosition ? 0 : yPivot1Minus);
                    if (isOn)
                        wPosMoveTo = -wCanvas / 2 + width * (sheet.isPivotEffectPosition ? 0 : xPivot);
                    else
                        wPosMoveTo = -wCanvas / 2 - width * (sheet.isPivotEffectPosition ? 1 : xPivot1Minus);
                    break;
                case ScreenPosition.TOPRIGHT:
                    hPosMoveTo = hCanvas / 2 - height * (sheet.isPivotEffectPosition ? 0 : yPivot1Minus);
                    if (isOn)
                        wPosMoveTo = wCanvas / 2 - width * (sheet.isPivotEffectPosition ? 0 : xPivot1Minus);
                    else
                        wPosMoveTo = wCanvas / 2 + width * (sheet.isPivotEffectPosition ? 1 : xPivot);
                    break;
                case ScreenPosition.BOTTOMLEFT:
                    hPosMoveTo = -hCanvas / 2 + height * (sheet.isPivotEffectPosition ? 0 : yPivot);
                    if (isOn)
                        wPosMoveTo = -wCanvas / 2 + width * (sheet.isPivotEffectPosition ? 0 : xPivot);
                    else
                        wPosMoveTo = -wCanvas / 2 - width * (sheet.isPivotEffectPosition ? 1 : xPivot1Minus);
                    break;
                case ScreenPosition.BOTTOMRIGHT:
                    hPosMoveTo = -hCanvas / 2 + height * (sheet.isPivotEffectPosition ? 0 : yPivot);
                    if (isOn)
                        wPosMoveTo = wCanvas / 2 - width * (sheet.isPivotEffectPosition ? 0 : xPivot1Minus);
                    else
                        wPosMoveTo = wCanvas / 2 + width * (sheet.isPivotEffectPosition ? 1 : xPivot);
                    break;
            }

            if (overlay != null)
                overlay.gameObject.SetActive(isOn);

            if (!activeSelfSheet) sheet.canvasGroup.gameObject.SetActive(isOn);

            Vector3 targetPos = new Vector3(wPosMoveTo, hPosMoveTo, 0) + (Vector3)sheet.offset;
#if LITMOTION_ENABLED
            LMotion.Create(sheet.canvasGroup.transform.localPosition, targetPos, duration).WithScheduler(MotionScheduler.UpdateIgnoreTimeScale).BindToLocalPosition(sheet.canvasGroup.transform);
            await LMotion.Create(sheet.canvasGroup.alpha, isOn ? 1 : 0, duration * 2).WithScheduler(MotionScheduler.UpdateIgnoreTimeScale).BindToAlpha(sheet.canvasGroup).ToUniTask();
#elif DOTWEEN_ENABLED
            sheet.canvasGroup.transform.DOLocalMove(targetPos, duration).SetUpdate(true);
            await sheet.canvasGroup.DOFade(isOn ? 1 : 0, duration * 2).SetUpdate(true).AsyncWaitForCompletion();
#else
            sheet.canvasGroup.transform.localPosition = targetPos;
            sheet.canvasGroup.alpha = isOn ? 1 : 0;
            await UniTask.CompletedTask;
#endif

            sheet.canvasGroup.blocksRaycasts = isOn;
            if (!activeSelfSheet) sheet.canvasGroup.gameObject.SetActive(isOn);
        }

    }

    [Serializable]
    public class SheetContainerType
    {
        public RectTransform rectTransform;
        public CanvasGroup canvasGroup;
        public Toggle toggle;
        public ScreenPosition position;
        public Button cancelButton;
        public bool isPivotEffectPosition = false;
        public Vector2 offset;
    }

    public enum ScreenPosition
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        TOPLEFT,
        TOPRIGHT,
        BOTTOMLEFT,
        BOTTOMRIGHT,
        NONE
    }
}