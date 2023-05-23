using System;
using Leopotam.EcsLite;

namespace td.features.shards.ui
{
    [Serializable]
    public struct ShardUIIsHovered
    {
        public EcsPackedEntity packedEntity;
    }
}