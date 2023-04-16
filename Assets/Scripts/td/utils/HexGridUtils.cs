using System;
using td.common;
using UnityEngine;

namespace td.utils
{
    public static class HexGridUtils
    {
        public static Vector2 CellToCoords(Int2 cell, float size) {
            var hexOffsetX = (size / 2f) * 1.5f;
            var hexOffsetY = (size / 2f) * 1.7320508f;
            
            var x = hexOffsetX * (cell.x + 0.5f);
            var y = hexOffsetY * (cell.y + (Math.Abs(cell.x) % 2) / 2f + 0.5f);
            return new Vector2 {x = x, y = y};
        }

        public static Int2 CoordsToCell(Vector2 coord, float size) {
            var hexOffsetX = (size / 2f) * 1.5f;
            var hexOffsetY = (size / 2f) * 1.7320508f;
            
            var x = Mathf.FloorToInt(coord.x / hexOffsetX);
            var y = Mathf.FloorToInt((coord.y - (Math.Abs(x) % 2) * hexOffsetY / 2) / hexOffsetY);
            return new Int2() { x = x, y = y };
        }

        public static Vector2 SnapToGrid(Vector2 position, float size) =>
            CellToCoords(CoordsToCell(position, size), size);
    }
}