using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace PirexGames.Core.UINavigator
{
    public class TooltipBaseAutoView : TooltipBaseView
    {
        [SerializeField] private RectTransform arrow;
        [SerializeField] private float containerPadding = 10f;
        [SerializeField] private float targetSpacing = 5f;

        public TooltipPosition positionMode = TooltipPosition.Auto;
        public TooltipPosition allowedAutoPositions = TooltipPosition.Top | TooltipPosition.Bottom | TooltipPosition.Left | TooltipPosition.Right;

        internal async UniTask InitAuto(string content, RectTransform target, RectTransform container, Vector2 offset)
        {
            View.alpha = 0;
            gameObject.SetActive(true);

            if (contentTxt != null) contentTxt.text = content;

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

            RectTransform tooltipRect = (RectTransform)transform;
            Vector2 tooltipSize = tooltipRect.rect.size;

            Vector3[] targetCorners = new Vector3[4];
            target.GetWorldCorners(targetCorners);
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            
            for (int i = 0; i < 4; i++)
            {
                Vector2 localPoint = container.InverseTransformPoint(targetCorners[i]);
                minX = Mathf.Min(minX, localPoint.x);
                maxX = Mathf.Max(maxX, localPoint.x);
                minY = Mathf.Min(minY, localPoint.y);
                maxY = Mathf.Max(maxY, localPoint.y);
            }

            Vector2 targetCenter = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
            Rect cRect = container.rect;
            float limMinX = cRect.xMin + containerPadding;
            float limMaxX = cRect.xMax - containerPadding;
            float limMinY = cRect.yMin + containerPadding;
            float limMaxY = cRect.yMax - containerPadding;

            float spaceTop = limMaxY - maxY;
            float spaceBottom = minY - limMinY;
            float spaceRight = limMaxX - maxX;
            float spaceLeft = minX - limMinX;

            TooltipPosition bestVertical = spaceTop >= spaceBottom ? TooltipPosition.Top : TooltipPosition.Bottom;
            TooltipPosition worstVertical = spaceTop >= spaceBottom ? TooltipPosition.Bottom : TooltipPosition.Top;
            
            TooltipPosition bestHorizontal = spaceRight >= spaceLeft ? TooltipPosition.Right : TooltipPosition.Left;
            TooltipPosition worstHorizontal = spaceRight >= spaceLeft ? TooltipPosition.Left : TooltipPosition.Right;

            TooltipPosition[] priorityOrder = { bestVertical, worstVertical, bestHorizontal, worstHorizontal };
            (bool fits, Vector2 center, TooltipPosition side) best = (false, Vector2.zero, TooltipPosition.None);

            foreach (TooltipPosition s in priorityOrder)
            {
                if (positionMode == TooltipPosition.Auto && (allowedAutoPositions & s) == 0) continue;
                if (positionMode != TooltipPosition.Auto && positionMode != s) continue;

                Vector2 testCenter = targetCenter;
                if (s == TooltipPosition.Top) testCenter.y = maxY + targetSpacing + (tooltipSize.y * 0.5f);
                else if (s == TooltipPosition.Bottom) testCenter.y = minY - targetSpacing - (tooltipSize.y * 0.5f);
                else if (s == TooltipPosition.Right) testCenter.x = maxX + targetSpacing + (tooltipSize.x * 0.5f);
                else if (s == TooltipPosition.Left) testCenter.x = minX - targetSpacing - (tooltipSize.x * 0.5f);

                var res = CheckAndClampCenter(testCenter, tooltipSize, limMinX, limMaxX, limMinY, limMaxY, s == TooltipPosition.Right || s == TooltipPosition.Left);
                
                if (best.side == TooltipPosition.None) best = (false, res.center, s);
                
                if (res.fits) 
                { 
                    best = (true, res.center, s); 
                    break; 
                }
            }

            if (best.side == TooltipPosition.None)
            {
                best.side = priorityOrder[0];
                best.center = targetCenter;
                
                if (best.side == TooltipPosition.Top) best.center.y = maxY + targetSpacing + (tooltipSize.y * 0.5f);
                else if (best.side == TooltipPosition.Bottom) best.center.y = minY - targetSpacing - (tooltipSize.y * 0.5f);
                else if (best.side == TooltipPosition.Right) best.center.x = maxX + targetSpacing + (tooltipSize.x * 0.5f);
                else if (best.side == TooltipPosition.Left) best.center.x = minX - targetSpacing - (tooltipSize.x * 0.5f);
            }

            Vector2 arrowRelCenter = Vector2.zero;
            float clampX = (tooltipSize.x * 0.5f) - containerPadding;
            float clampY = (tooltipSize.y * 0.5f) - containerPadding;

            if (best.side == TooltipPosition.Top || best.side == TooltipPosition.Bottom)
            {
                arrowRelCenter.x = Mathf.Clamp(targetCenter.x - best.center.x, -clampX, clampX);
                arrowRelCenter.y = (best.side == TooltipPosition.Top) ? -tooltipSize.y * 0.5f : tooltipSize.y * 0.5f;
            }
            else
            {
                arrowRelCenter.y = Mathf.Clamp(targetCenter.y - best.center.y, -clampY, clampY);
                arrowRelCenter.x = (best.side == TooltipPosition.Right) ? -tooltipSize.x * 0.5f : tooltipSize.x * 0.5f;
            }

            Vector2 newPivot = new Vector2(
                0.5f + (arrowRelCenter.x / tooltipSize.x),
                0.5f + (arrowRelCenter.y / tooltipSize.y)
            );
            
            tooltipRect.pivot = newPivot;

            if (arrow != null)
            {
                float rotation = 0;
                if (best.side == TooltipPosition.Top) rotation = 0;
                else if (best.side == TooltipPosition.Bottom) rotation = 180;
                else if (best.side == TooltipPosition.Right) rotation = -90;
                else if (best.side == TooltipPosition.Left) rotation = 90;

                arrow.localRotation = Quaternion.Euler(0, 0, rotation);
                arrow.anchorMin = new Vector2(0.5f, 0.5f);
                arrow.anchorMax = new Vector2(0.5f, 0.5f);
                arrow.anchoredPosition = arrowRelCenter;
            }

            Vector2 finalPos = best.center + arrowRelCenter + offset;

            await Init(content, finalPos, true);
            View.alpha = 1;
        }

        private (bool fits, Vector2 center) CheckAndClampCenter(Vector2 center, Vector2 size, float minX, float maxX, float minY, float maxY, bool isHorizontal)
        {
            Vector2 clamped = center;
            Vector2 halfSize = size * 0.5f;

            if (isHorizontal)
            {
                clamped.y = Mathf.Clamp(center.y, minY + halfSize.y, maxY - halfSize.y);
            }
            else
            {
                clamped.x = Mathf.Clamp(center.x, minX + halfSize.x, maxX - halfSize.x);
            }

            bool fits = (clamped.x - halfSize.x >= minX) && (clamped.x + halfSize.x <= maxX) &&
                        (clamped.y - halfSize.y >= minY) && (clamped.y + halfSize.y <= maxY);

            return (fits, clamped);
        }
    }
}