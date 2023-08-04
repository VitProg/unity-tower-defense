using td.common;
using UnityEngine;
using UnityEngine.Rendering;

namespace td.utils
{
    public static class RandomUtils
    {
        public static int IntRange(int min, int max) =>
            Random.Range(min, max + 1);

        public static float Range(MinMaxF minMax) =>
            Range(minMax.min, minMax.max);

        public static float Range(float[] minMax) =>
            Range(minMax[0], minMax[1]);

        public static float Range(float min, float max) =>
            Random.Range(min * 100f, max * 100f) / 100f;

        public static T RandomArrayItem<T>(ref T[] array) =>
            array[IntRange(0, array.Length - 1)];


        public static Vector2 Vector2(float[] minMax) =>
            Vector2(minMax[0], minMax[1]);

        public static Vector2 Vector2(MinMaxF minMax) =>
            Vector2(minMax.min, minMax.max);
        
        public static Vector2 Vector2(float min, float max) =>
            new Vector2(Range(min, max),Range(min, max));

        public static bool Bool(float probability = 0.5f) => Random.Range(0f, 1f) >= probability;

        public static Quaternion Rotation() => RandomArrayItem(ref rotations);
        
        private static Quaternion[] rotations = new Quaternion[360*2];
        static RandomUtils()
        {
            var index = 0;
            for (var angl = 0f; angl < 360; angl += 0.5f)
            {
                rotations[index] = Quaternion.AngleAxis(angl, Vector3.forward);
                index++;
            }

            Debug.Log(rotations);
            
        }
    }
}