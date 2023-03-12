using UnityEngine;

namespace td.utils
{
    public static class RandomUtils
    {
        public static int IntRange(int min, int max) =>
            Mathf.RoundToInt(Random.Range((float)min, (float)max));

        public static float Range(float min, float max) =>
            Random.Range(min, max);

        public static T RandomArrayItem<T>(T[] array) =>
            array[IntRange(0, array.Length - 1)];
    }
}