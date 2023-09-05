using System.Runtime.CompilerServices;
using UnityEngine;

namespace td.utils
{
    public static class RotateUtils
    {
        private const float Step = 0.25f;
        private const int ItemsCount = 360 * (int)(1 / Step);
        private static readonly Quaternion[] RotationsCache = new Quaternion[ItemsCount];

        public static Quaternion Random(float minAngle, float maxAngle)
        {
            var fromIndex = GetIndex(minAngle);
            var toIndex = GetIndex(maxAngle);
            return toIndex <= fromIndex
                ? Quaternion.identity
                : RotationsCache[RandomUtils.IntRange(fromIndex, toIndex)];
        }

        public static Quaternion Random() => RotationsCache[RandomUtils.IntRange(0, ItemsCount)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetIndex(float angle)
        {
            angle %= 360f;
            if (angle < 0) angle += 360f;
            // Вычисление ближайшего индекса в массиве rotationsCache
            return Mathf.RoundToInt(angle / Step) % ItemsCount;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion GetByAngle(float angle) => RotationsCache[GetIndex(angle)];
        
        static RotateUtils()
        {
            var index = 0;
            for (var angl = 0f; angl < 360; angl += Step)
            {
                RotationsCache[index] = Quaternion.AngleAxis(angl, Vector3.forward);
                index++;
            }
        }
    }
}