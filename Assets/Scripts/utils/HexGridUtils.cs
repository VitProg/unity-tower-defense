using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace td.utils
{
    public static class HexGridUtils
    {
        public const float HexOffsetX = 0.75f;
        public const float HexOffsetY = 0.8660254f;
        public const float SkewY = 0.85f;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector2 CellToPosition(int2 cell) => CellToPosition(cell.x, cell.y);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector2 CellToPosition(ref int2 cell) => CellToPosition(cell.x, cell.y);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector2 CellToPosition(int cellX, int cellY)
        {
            var x = HexOffsetX * (cellX + 0.5f);
            var y = HexOffsetY * (cellY + (Math.Abs(cellX) % 2) / 2f + 0.5f) * SkewY;
            return new Vector2 { x = x, y = y };
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static int2 PositionToCell(Vector2 position)
        {
            var x = Mathf.FloorToInt(position.x / HexOffsetX);
            var y = Mathf.FloorToInt(((position.y / SkewY) - (Math.Abs(x) % 2) * HexOffsetY / 2) / HexOffsetY);
            return new int2() { x = x, y = y };
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static int2 PositionToCell(float posX, float posY)
        {
            var x = Mathf.FloorToInt(posX / HexOffsetX);
            var y = Mathf.FloorToInt(((posY / SkewY) - (Math.Abs(x) % 2) * HexOffsetY / 2) / HexOffsetY);
            return new int2(x, y);
        }

        public static Vector2 SnapToGrid(Vector2 position) =>
            CellToPosition(PositionToCell(position));

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static int2 GetNeighborsCoords(ref int2 cellCoords, HexDirections direction)
        {
            var x = cellCoords.x;
            var y = cellCoords.y;

            var isOddRow = Math.Abs(x) % 2 == 1;

            return direction switch
            {
                HexDirections.NorthWest => isOddRow ? new int2(x - 1, y + 1) : new int2(x - 1, y + 0),
                HexDirections.North => new int2(x + 0, y + 1),
                HexDirections.NorthEast => isOddRow ? new int2(x + 1, y + 1) : new int2(x + 1, y + 0),
                HexDirections.SouthEast => isOddRow ? new int2(x + 1, y - 0) : new int2(x + 1, y - 1),
                HexDirections.South => new int2(x + 0, y - 1),
                HexDirections.SouthWest => isOddRow ? new int2(x - 1, y - 0) : new int2(x - 1, y - 1),
                _ => new int2(0, 0)
            };
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static HexDirections GetDirection(ref int2 a, ref int2 b)
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

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
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