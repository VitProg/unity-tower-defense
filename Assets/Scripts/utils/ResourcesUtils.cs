using UnityEngine;

namespace td.utils
{
    public static class ResourcesUtils
    {
        public static T LoadJson<T>(string path)
        {
            var textAsset = Resources.Load<TextAsset>(path);
            return JsonUtility.FromJson<T>(textAsset.text);
        }
    }
}