using System;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    static class DrawOverlayIcon
    {
        static class OverlayRect
        {
            const int k_OverlayStatusIconSize = 16;
            const int k_UnityStandardIconSize = 32;
            const int k_OverlayIconHorizontalOffset = 2;
            const int k_OverlayIconVerticalOffset = 16; // This is the constant offset from the asset's name text below the thumbnail.

            internal static Rect GetOverlayRect(Rect selectionRect, ProjectIconOverlayPosition iconOverlayPosition)
            {
                var widthRatio = selectionRect.width / k_UnityStandardIconSize;
                var heightRatio = selectionRect.height / k_UnityStandardIconSize;

                var overlayIconSize = new Vector2(k_OverlayStatusIconSize * widthRatio / 2f, k_OverlayStatusIconSize * heightRatio / 2f);
                var overlayIconOffset = new Vector2(k_OverlayIconHorizontalOffset * widthRatio, k_OverlayIconVerticalOffset);

                float xPos, yPos;
                switch (iconOverlayPosition)
                {
                    case ProjectIconOverlayPosition.TopRight:
                        xPos = selectionRect.x + selectionRect.width - overlayIconSize.x - overlayIconOffset.x;
                        yPos = selectionRect.y;
                        break;
                    case ProjectIconOverlayPosition.TopLeft:
                        xPos = selectionRect.x + overlayIconOffset.x;
                        yPos = selectionRect.y;
                        break;
                    case ProjectIconOverlayPosition.BottomLeft:
                        xPos = selectionRect.x + overlayIconOffset.x;
                        yPos = selectionRect.y + selectionRect.height - overlayIconSize.y - overlayIconOffset.y;
                        break;
                    case ProjectIconOverlayPosition.BottomRight:
                        xPos = selectionRect.x + selectionRect.width - overlayIconSize.x - overlayIconOffset.x;
                        yPos = selectionRect.y + selectionRect.height - overlayIconSize.y - overlayIconOffset.y;
                        break;
                    default:
                        xPos = selectionRect.x;
                        yPos = selectionRect.y;
                        break;

                }

                return new Rect(
                    xPos,
                    yPos,
                    overlayIconSize.x,
                    overlayIconSize.y);
            }
        }

        internal static void ForStatus(Rect selectionRect, ProjectIconAssetStatus status, ProjectIconOverlayPosition overlayPosition)
        {
            var overlayIcon = UIElementsUtils.GetTexture(status.IconPath);
            if (overlayIcon == null)
                return;

            var overlayRect = OverlayRect.GetOverlayRect(selectionRect, overlayPosition);

            GUI.DrawTexture(overlayRect, overlayIcon, ScaleMode.ScaleToFit);

            var tooltipRect = GetTooltipRect(selectionRect, overlayRect);

            GUI.Label(tooltipRect, new GUIContent(string.Empty, L10n.Tr(status.Description)));
        }

        static Rect Inflate(Rect rect, float width, float height)
        {
            return new Rect(
                rect.x - width,
                rect.y - height,
                rect.width + 2f * width,
                rect.height + 2f * height);
        }

        static Rect GetTooltipRect(Rect selectionRect, Rect overlayRect)
        {
            return selectionRect.width > selectionRect.height ? overlayRect : Inflate(overlayRect, 3f, 3f);
        }
    }
}
