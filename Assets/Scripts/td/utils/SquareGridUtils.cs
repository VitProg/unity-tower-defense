using td.common;
using UnityEngine;

namespace td.utils
{
    public static class SquareGridUtils
    {
        public static Int2 CoordsToCell(Vector2 position, float size) =>
            new(
                Mathf.RoundToInt(position.x / size),
                Mathf.RoundToInt(position.y / size)
            );

        public static Vector2 CellToCoords(Int2 coordinate, float size) =>
            new(
                coordinate.x * size,
                coordinate.y * size
            );

        public static Vector2 SnapToGrid(Vector2 position, float size) =>
            CellToCoords(CoordsToCell(position, size), size);
    }
}