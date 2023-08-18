using System;
using UnityEngine;

namespace td.features.building.data
{
    [CreateAssetMenu(menuName = "TD/BuildingsConfig")]
    public class Buildings_Config_SO : ScriptableObject
    {
        public Sprite shardTowerIcon;
        public Sprite shardTrapIcon;
        public Sprite generatorIcon;
        public Sprite energyStorageIcon;
        public Sprite solarPowerIcon;

        public Sprite GetIcon(string buildingId) =>
            buildingId switch
            {
                "shard_tower" => shardTowerIcon,
                "shard_trap" => shardTrapIcon,
                "generator" => generatorIcon,
                "energy_storage" => energyStorageIcon,
                "solar_power" => solarPowerIcon,
#if UNITY_EDITOR
                _ => throw new Exception($"Icon for building '{buildingId}' not found")
#endif
            };
    }
}