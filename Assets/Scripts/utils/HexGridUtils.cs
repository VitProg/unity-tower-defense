using System;
using td.common;
using UnityEngine;

namespace td.utils
{
    public static class HexGridUtils
    {
        public const float HexOffsetX = 0.75f;
        public const float HexOffsetY = 0.8660254f;
        public const float SkewY = 0.85f;

        public static Vector2 CellToPosition(Int2 cell)
        {
            var x = HexOffsetX * (cell.x + 0.5f);
            var y = HexOffsetY * (cell.y + (Math.Abs(cell.x) % 2) / 2f + 0.5f) * SkewY;
            return new Vector2 { x = x, y = y };
        }

        public static Int2 PositionToCell(Vector2 position)
        {
            var x = Mathf.FloorToInt(position.x / HexOffsetX);
            var y = Mathf.FloorToInt(((position.y / SkewY) - (Math.Abs(x) % 2) * HexOffsetY / 2) / HexOffsetY);
            return new Int2() { x = x, y = y };
        }

        public static Vector2 SnapToGrid(Vector2 position) =>
            CellToPosition(PositionToCell(position));

        public static Int2 GetNeighborsCoords(Int2 cellCoords, HexDirections direction)
        {
            var x = cellCoords.x;
            var y = cellCoords.y;

            var isOddRow = Math.Abs(x) % 2 == 1;

            return direction switch
            {
                HexDirections.NorthWest => isOddRow ? new Int2(x - 1, y + 1) : new Int2(x - 1, y + 0),
                HexDirections.North => new Int2(x + 0, y + 1),
                HexDirections.NorthEast => isOddRow ? new Int2(x + 1, y + 1) : new Int2(x + 1, y + 0),
                HexDirections.SouthEast => isOddRow ? new Int2(x + 1, y - 0) : new Int2(x + 1, y - 1),
                HexDirections.South => new Int2(x + 0, y - 1),
                HexDirections.SouthWest => isOddRow ? new Int2(x - 1, y - 0) : new Int2(x - 1, y - 1),
                _ => new Int2(0, 0)
            };
        }

        public static HexDirections GetDirection(Int2 a, Int2 b)
        {
            var xDiff = b.x - a.x;
            var yDiff = b.y - a.y;
            var isOddRow = Math.Abs(a.x) % 2 == 1;

            if ((xDiff == -1 && yDiff == 1 && isOddRow) || (xDiff == -1 && yDiff == 0 && !isOddRow))
                return HexDirections.NorthWest;

            if (xDiff == 0 && yDiff == 1)
                return HexDirections.North;

            if ((xDiff == 1 && yDiff == 1 && isOddRow) || (xDiff == 1 && yDiff == 0 && !isOddRow))
                return HexDirections.NorthEast;

            if ((xDiff == 1 && yDiff == 0 && isOddRow) || (xDiff == 1 && yDiff == -1 && !isOddRow))
                return HexDirections.SouthEast;

            if (xDiff == 0 && yDiff == -1)
                return HexDirections.South;

            if ((xDiff == -1 && yDiff == 0 && isOddRow) || (xDiff == -1 && yDiff == -1 && !isOddRow))
                return HexDirections.SouthWest;

            return HexDirections.NONE;
        }

        public static HexDirections ReverseDirection(HexDirections direction) =>
            direction switch
            {
                HexDirections.NorthWest => HexDirections.SouthEast,
                HexDirections.North => HexDirections.South,
                HexDirections.NorthEast => HexDirections.SouthWest,
                HexDirections.SouthEast => HexDirections.NorthWest,
                HexDirections.South => HexDirections.North,
                HexDirections.SouthWest => HexDirections.NorthEast,
                _ => HexDirections.NONE
            };
    }

    public enum HexDirections
    {
        NONE = 0,
        NorthWest = 1,
        North = 2,
        NorthEast = 3,
        SouthEast = 4,
        South = 5,
        SouthWest = 6,
    }
}