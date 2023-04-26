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

        public static Vector2 CellToPosition(Int2 cell) {
            var x = HexOffsetX * (cell.x + 0.5f);
            var y = HexOffsetY * (cell.y + (Math.Abs(cell.x) % 2) / 2f + 0.5f) * SkewY;
            return new Vector2 {x = x, y = y};
        }

        public static Int2 PositionToCell(Vector2 position) {
            
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
        NONE,
        NorthWest,
        North,
        NorthEast,
        SouthEast,
        South,
        SouthWest,
    }
}