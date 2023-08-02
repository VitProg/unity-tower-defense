#if DEBUG && UNITY_EDITOR
using UnityEngine;

namespace td.utils
{
    public static class DebugEx
    {
        public static void DrawCircle(Vector2 center, float radius, Color color, int segments = 16, float duration = 1f, float yScale = 1f)
        {
            var angleStep = 360f / segments;

            var prevPoint = new Vector3(center.x + radius, center.y, 0f);

            for (var i = 1; i <= segments; i++)
            {
                var angle = i * angleStep;
                var x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                var y = center.y + (radius * Mathf.Sin(angle * Mathf.Deg2Rad)) * yScale;

                var currentPoint = new Vector3(x, y, 0f);
                Debug.DrawLine(prevPoint, currentPoint, color, duration, false);

                prevPoint = currentPoint;
            }
        }

        public static void DrawCross(Vector2 center, float size, Color color, float duration)
        {
            var s = size / 2f;
            var u = new Vector3(center.x, center.y - s, 0f);
            var d = new Vector3(center.x, center.y + s, 0f);
            var r = new Vector3(center.x - s, center.y, 0f);
            var l = new Vector3(center.x + s, center.y, 0f);
            Debug.DrawLine(u, d, color, duration, false);
            Debug.DrawLine(r, l, color, duration, false);
        }
    }
}
#endif