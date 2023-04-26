using System;
using System.Collections.Generic;

namespace td.utils
{
    public static class ArrayUtils
    {
        public static T[] Filter<T>(IEnumerable<T?> arr, Func<T?, bool> condition) where T : struct
        {
            var result = new List<T>();
            foreach (var item in arr)
            {
                if (condition(item))
                {
                    result.Add((T)item);
                }
            }

            return result.ToArray();
        }

        public static T[] NotNullable<T>(IEnumerable<T?> arr) where T : struct =>
            Filter(arr, IsNotNull);

        public static int[] GetIndexes<T>(IEnumerable<T?> arr, Func<T?, bool> condition) where T : struct
        {
            var result = new List<int>();
            var index = 0;
            foreach (var item in arr)
            {
                if (condition(item))
                {
                    result.Add(index);
                }

                index++;
            }

            return result.ToArray();
        }

        public static bool IsNotNull<T>(T? value) where T : struct =>
            value != null;
    }
}