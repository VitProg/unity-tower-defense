using System;
using UnityEngine;
using UnityEngine.UI;

namespace td.utils
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height
        }

        public FitType fitType;
        public int rows;
        public int columns;
        public Vector2 cellSize;
        public Vector2 spacing;
        public Rect pading;
        
        public override void CalculateLayoutInputVertical()
        {
            var childCount = transform.childCount;
            var rect = rectTransform.rect;
            
            var sqrRt = MathF.Sqrt(childCount);
            rows = (int)MathF.Ceiling(sqrRt);
            columns = (int)MathF.Ceiling(sqrRt);

            var fRows = (float)rows;
            var fColumns = (float)columns;
            
            if (fitType == FitType.Width)
            {
                rows = (int)MathF.Ceiling(childCount / fColumns);
            }

            if (fitType == FitType.Height)
            {
                columns = (int)MathF.Ceiling(childCount / fRows);
            }

            var parentWidth = rect.width;
            var parentHeight = rect.height;

            var cellWidth = (parentWidth / fColumns) - (spacing.x / fColumns * 2) - (pading.xMin / fColumns) - (pading.xMax / fColumns);
            var cellHeight = (parentHeight / fRows) - (spacing.y / fRows * 2) - (pading.yMin / fRows) - (pading.yMax / fRows);

            cellSize.x = cellWidth;
            cellSize.y = cellHeight;

            for (var i = 0; i < rectChildren.Count; i++)
            {
                var item = rectChildren[i];
                
                var rowCount = i / columns;
                var columnCount = i % columns;

                var xPos = cellSize.x * columnCount + spacing.x * columnCount + pading.xMin;
                var yPos = cellSize.y * rowCount + spacing.y * rowCount + pading.yMin;
                
                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
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