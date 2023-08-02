using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace td.features.ui
{
    [ExecuteAlways]
    public class UI_PopupBottomArrow : MonoBehaviour
    {
        public RectTransform rectTransform;

        public Image imgTopLeftCorner;
        public Image imgTopRightCorner;
        public Image imgBottomLeftCorner;
        public Image imgBottomRightCorner;
        public Image imgLeft;
        public Image imgRight;
        public Image imgTopLeft;
        public Image imgTopLeftInnerCorner;
        public Image imgTopRight;
        public Image imgTopRightInnerCorner;
        public Image imgTopCenter;
        public Image imgBottomLeft;
        public Image imgBottomLeftInner;
        public Image imgBottomLeftInnerCorner;
        public Image imgBottomRight;
        public Image imgBottomRightInner;
        public Image imgBottomRightInnerCorner;
        public Image imgBottomCenterArrow;
        public Image imgBg;

        private const float LeftRightWidth = 44f;
        private const float TopHeight = 65f;
        private const float TopInnerCornerWidth = 36f;
        private const float BottomInnerCornerWidth = 36f;
        private const float BottomArrowWidth = 80f;
        private const float BottomHeight = 80f;

        public float topCenterMinWidth = 20f;

        private void Update()
        {
            if (!transform.hasChanged || !rectTransform) return;
            
            var rect = rectTransform.rect;

            var width = rect.width;
            var halfWidth = width / 2f;
            var height = rect.height;
            var halfHeight = height / 2f;

            var topCenterWidthW = Mathf.Max(topCenterMinWidth, (width - LeftRightWidth * 2f - TopInnerCornerWidth * 2f) / 2f);
            var topRightInnerCornerX = topCenterWidthW / 2f;
            var topLeftInnerCornerX = topRightInnerCornerX * -1;
            var topLeftRightW = halfWidth - LeftRightWidth - TopInnerCornerWidth - topCenterWidthW / 2f;

            var bottomW = halfWidth - LeftRightWidth - BottomArrowWidth / 2f - BottomInnerCornerWidth;
            var bottomWOuter = bottomW / 1.4f;
            var bottomWInner = bottomW - bottomWOuter;
            var bottomRightInnerCornerX = (BottomArrowWidth / 2f) + bottomWInner;
            var bottomLeftInnerCornerX = bottomRightInnerCornerX * -1;

            imgTopCenter.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, topCenterWidthW);
            imgTopLeft.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, topLeftRightW);
            imgTopRight.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, topLeftRightW);
            imgTopLeftInnerCorner.rectTransform.anchoredPosition = new Vector2(topLeftInnerCornerX, 0f);
            imgTopRightInnerCorner.rectTransform.anchoredPosition = new Vector2(topRightInnerCornerX, 0f);

            imgBottomLeft.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bottomWOuter);
            imgBottomRight.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bottomWOuter);
            imgBottomLeftInnerCorner.rectTransform.anchoredPosition = new Vector2(bottomLeftInnerCornerX, 0f);
            imgBottomRightInnerCorner.rectTransform.anchoredPosition = new Vector2(bottomRightInnerCornerX, 0f);
            imgBottomLeftInner.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bottomWInner);
            imgBottomRightInner.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bottomWInner);
        }
    }
}