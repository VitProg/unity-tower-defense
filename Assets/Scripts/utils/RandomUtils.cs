using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace td.utils
{
    public static class RandomUtils
    {
        public static readonly System.Random Random;

        static RandomUtils()
        {
            Random = new System.Random(Environment.TickCount);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IntRange(int min, int max) =>
            Random.Next(min, max + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Range(float min = 0f, float max = 1f) {
            var randomValue = Random.Next();
            var normalizedValue = (float)randomValue / int.MaxValue;
            return normalizedValue * (max - min) + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomArrayItem<T>(ref T[] array) =>
            array[IntRange(0, array.Length - 1)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Vector2(float[] minMax) =>
            Vector2(minMax[0], minMax[1]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Vector2(float min, float max) =>
            new(Range(min, max),Range(min, max));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Bool(float probability = 0.5f) => Random.NextDouble() < probability;

    }
}