using System;
using UnityEngine.Rendering;

namespace td.utils
{
    public static class FloatUtils {
        private const float Inaccuracy = 0.0001f;
        
        public static bool IsEquals(float a, float b, float inaccuracy = Inaccuracy) => Math.Abs(a - b) < inaccuracy;
        public static bool IsZero(float a, float inaccuracy = Inaccuracy) => IsEquals(a, 0f, inaccuracy);

        public static float DefaultIfZero(float? value, float defaultValue = 0f, float inaccuracy = Inaccuracy) =>
            value == null ? defaultValue : (
                IsEquals(value.Value, 0f, inaccuracy) ? defaultValue : value.Value
            );
    }
}