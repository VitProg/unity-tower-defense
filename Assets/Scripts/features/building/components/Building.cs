using Unity.Mathematics;

namespace td.features.building.components
{
    public struct Building
    {
        public string id;
        public int2 coords;
        public uint price;
        public float extraFeaturePriceMultiplier;
        public float buildTimeRemaining;
        public float extraFeatureTimeRemaining;
    }
}