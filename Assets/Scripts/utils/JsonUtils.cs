using UnityEngine;

namespace td.utils
{
    public class JsonUtils
    {
        public static string ArrayToJson<T>(T[] array)
        {
            var wrapper = new Wrapper<T>
            {
                array = array
            };
            return JsonUtility.ToJson(wrapper);
        }
        
        public static T[] ArrayFromJson<T>(string json)
        {
            var newJson = "{ \"array\": " + json + "}";
            var wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }
        
        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}