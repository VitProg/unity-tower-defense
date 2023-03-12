using td.common;
using UnityEngine;

namespace td.utils
{
    public static class GridUtils
    {
        public static Int2 GetGridCoordinate(Vector2 position) =>
            new(
                Mathf.RoundToInt(position.x / 2),
                Mathf.RoundToInt(position.y / 2)
            );

        public static Vector2 GetVector(Int2 coordinate) =>
            new(
                coordinate.x * 2.0f,
                coordinate.y * 2.0f
            );
    }
}