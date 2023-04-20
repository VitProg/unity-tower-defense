using td.common;
using UnityEngine;

namespace td.utils
{
    public static class SquareGridUtils
    {
        public static Int2 CoordsToCell(Vector2 position) =>
            new(
                Mathf.RoundToInt(position.x),
                Mathf.RoundToInt(position.y)
            );

        public static Vector2 CellToCoords(Int2 coordinate) =>
            new(
                coordinate.x,
                coordinate.y
            );

        public static Vector2 SnapToGrid(Vector2 position) =>
            CellToCoords(CoordsToCell(position));
    }
}