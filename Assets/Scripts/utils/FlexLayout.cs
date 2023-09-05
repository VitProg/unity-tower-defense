using NaughtyAttributes;
using UnityEngine.UI;

namespace td.utils
{
    public class FlexLayout : LayoutGroup
    {
        public enum Alignment
        {
            Start,
            Center,
            End,
        };
        
        public enum Direction
        {
            Horizontal,
            Vertical,
        }

        public Direction direction = Direction.Horizontal;
        public float spacingAbsolute;
        public float spacingPercent;
        public float itemAspect = 1f;
        public bool fix = false;
        public bool reverse = false;
        public Alignment alignment = Alignment.Center;

        [Button("RefreshLayout")]
        public override void CalculateLayoutInputVertical()
        {
            var childCount = transform.childCount;
            var rect = rectTransform.rect;

            var isSpacePercent = spacingPercent is > 0.0001f or < -0.0001f;
            
            var spacing = isSpacePercent ? rect.width * spacingPercent : spacingAbsolute;
            var halfSpacing = spacing / 2f;

            float width, height, size, rectSize, paddingStart, paddingEnd;

            if (direction == Direction.Horizontal) {
                height = rect.height - padding.top - padding.bottom;
                width = height / itemAspect;
                size = width;
                rectSize = rect.width;
                paddingStart = padding.left;
                paddingEnd = padding.right;
            } else {
                width = rect.width - padding.left - padding.right;
                height = width * itemAspect;
                size = height;
                rectSize = rect.height;
                paddingStart = padding.top;
                paddingEnd = padding.bottom;
            }
            
            var maxSize = rectSize - paddingStart - paddingEnd;
            var shift = 0f;
            var itemsSize = (size + spacing) * childCount;

            if (itemsSize > maxSize && fix) {
                var d = maxSize / itemsSize;

                spacing = spacing > 0 ? spacing * d : spacing / d * 1.5f;
                size = maxSize / childCount - spacing;

                if (direction == Direction.Horizontal) {
                    width = size;
                    height = width * itemAspect;
                    shift = rect.height / 2 - height / 2;
                } else {
                    height = size;
                    width = height / itemAspect;
                    shift = rect.width / 2 - width / 2;
                }
                
                itemsSize = (size + spacing) * childCount;
            }
            
            var itemsHalfSize = itemsSize / 2f;

            for (var index = 0; index < childCount; index++)
            {
                var item = rectChildren[reverse ? childCount - index - 1 : index];

                var coord = index * (size + spacing);

                switch (alignment)
                {
                    case Alignment.Start:
                        coord += paddingStart;
                        break;
                    case Alignment.End:
                        coord = coord + (rect.width - itemsSize) - paddingEnd;
                        break;
                    case Alignment.Center:
                    default:
                        coord = coord - itemsHalfSize + rectSize / 2f + halfSpacing;
                        break;
                }

                if (direction == Direction.Horizontal)
                {
                    SetChildAlongAxis(item, 0, coord, width);
                    SetChildAlongAxis(item, 1, padding.top + shift, height);
                }
                else
                {
                    SetChildAlongAxis(item, 0, padding.left + shift, width);
                    SetChildAlongAxis(item, 1, coord, height);
                }
            }
        }

        public override void SetLayoutHorizontal()
        {
            
        }

        public override void SetLayoutVertical()
        {
            
        }
    }
}