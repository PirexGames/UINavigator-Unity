using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
namespace Pirex.UI.Base
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

        private void Start()
        {
            foreach (var item in sheets)
            {
                if (isAutoOperation)
                    item.toggle.onValueChanged.AddListener(delegate { TransitionOn(item.toggle); });
                if (item.cancelButton != null)
                    item.cancelButton.onClick.AddListener(TransitionOff);

            }
            if (!allowSwitchOff)
            {
                TransitionOn(sheets[0].toggle);
            }
        }

        public void TransitionOn(RectTransform rectTransform)
        {
            SheetContainerType sheet = FindSheet(rectTransform);
            if (sheet.toggle.isOn)
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
            foreach (var item in sheets)
            {
                item.toggle.isOn = true;
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
            if (activeSelfSheet)
                activeSheet.toggle.isOn = false;
        }

        public void TransitionOffAll()
        {
            if (!allowMultiActive) return;
            foreach (var item in sheets)
            {
                item.toggle.isOn = false;
            }
        }
        private SheetContainerType FindSheet(Toggle toggle)
        {
            foreach (var item in sheets)
            {
                if (item.toggle == toggle)
                    return item;
            }
            return null;
        }

        private SheetContainerType FindSheet(RectTransform rect)
        {
            foreach (var item in sheets)
            {
                if (item.rectTransform == rect)
                    return item;
            }
            return null;
        }

        public async UniTask TransitionSheetAsync(SheetContainerType sheet, bool isOn)
        {
            var sheetRect = sheet.rectTransform;
            float xPivot = sheetRect.pivot.x;
            float yPivot = sheetRect.pivot.y;

            float xPivot1Minus = 1 - xPivot;
            float yPivot1Minus = 1 - yPivot;

            float yPivotMinus05 = yPivot - 0.5f;
            float xPivotMinus05 = xPivot - 0.5f;

            float width = sheetRect.rect.width;
            float height = sheetRect.rect.height;

            float wCanvas = canvas.rect.width;
            float hCanvas = canvas.rect.height;


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

            sheet.canvasGroup.transform.DOLocalMove(new Vector2(wPosMoveTo, hPosMoveTo) + sheet.offset, duration);
            await sheet.canvasGroup.DOFade(isOn ? 1 : 0, duration * 2).AsyncWaitForCompletion();

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