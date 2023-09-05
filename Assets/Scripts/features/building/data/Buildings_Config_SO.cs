using System;
using td.utils;
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
                Constants.Buildings.ShardTower => shardTowerIcon,
                Constants.Buildings.ShardTrap => shardTrapIcon,
                Constants.Buildings.Generator => generatorIcon,
                Constants.Buildings.EnergyStorage => energyStorageIcon,
                Constants.Buildings.SolarPower => solarPowerIcon,
#if UNITY_EDITOR
                _ => throw new Exception($"Icon for building '{buildingId}' not found")
#endif
            };
    }
}