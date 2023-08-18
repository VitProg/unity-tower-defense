using System;
using UnityEngine;

namespace td.features.building.data
{
    [Serializable]
    public struct Building_Config_Collection
    {
        public Building_Config[] buildings;
    }

    [Serializable]
    public struct Building_Config
    {
        public string id;
        public string name;
        public string description;
        public string prefabName;
        public GameObject prefab;
        public uint price;
        public float priceIncrease;
        public uint buildTime;
        public float buildTimeIncrease;
        public float extraFeaturePrice;
        public float extraFeaturePriceIncrease;
        public float extraFeatureTime;
        public float extraFeatureTimeIncrease;
    }
}