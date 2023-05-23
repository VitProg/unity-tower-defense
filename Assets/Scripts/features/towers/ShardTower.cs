using System;
using Leopotam.EcsLite;

namespace td.features.towers
{
    [Serializable]
    public struct ShardTower
    {
        public float fireCountdown;
        public EcsPackedEntity shard;
    }
}