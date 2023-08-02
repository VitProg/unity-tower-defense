using System;
using UnityEngine.Rendering;

namespace td.utils
{
    public static class FloatUtils
    {
        public static bool IsEquals(float a, float b) => Math.Abs(a - b) < Constants.ZeroFloat;
        public static bool IsZero(float a) => IsEquals(a, 0f);

        public static float DefaultIfZero(float? value, float defaultValue = 0f) =>
            value == null ? defaultValue : (
                IsEquals(value.Value, 0f) ? defaultValue : value.Value
            );
    }
}