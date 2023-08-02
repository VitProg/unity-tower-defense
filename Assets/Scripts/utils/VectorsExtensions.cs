using System.Runtime.CompilerServices;
using UnityEngine;

namespace td.utils
{
    public static class VectorsExtensions
    {
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector2 v) => FloatUtils.IsZero(v.x) && FloatUtils.IsZero(v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this Vector2 ab, Vector2 cd) => ab.x * cd.x + ab.y * cd.y;
    }
}