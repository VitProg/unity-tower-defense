using System.Collections.Generic;
using UnityEngine;

namespace td.features._common
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Prefab_Service
    {
        private readonly Dictionary<string, GameObject> cache = new();

        public GameObject GetPrefab(PrefabCategory category, string name)
        {
            var key = $"{category}:{name}";

            if (cache.TryGetValue(key, out var prefab))
            {
                return prefab;
            }
            
            prefab = (GameObject)Resources.Load($"Prefabs/{category.ToString().ToLower()}/{name}", typeof(GameObject));
            cache.Add(key, prefab);

            return prefab;
        }
    }

    public enum PrefabCategory
    {
        Projectiles,
        Map,
        Enemies,
        Shard,
        Buildings,
        Windows,
        UI,
        FX,
    }
}