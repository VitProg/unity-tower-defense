using Unity.Mathematics;

namespace td.features.impactEnemy.components
{
    public struct ShockingDebuff
    {
        public float timeRemains;
        public float2 originalPosition;
        public float shiftPositionTimeRemains;
        public bool started;
    }
}