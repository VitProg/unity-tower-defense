using System;
using System.Runtime.CompilerServices;

namespace td.features.tower.components
{
    [Serializable]
    public struct ShardTower
    {
        public float fireCountdown;
        public float radius;
        public float sqrRadius;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRadius(float r)
        {
            radius = r;
            sqrRadius = r * r;
        }
    }
}