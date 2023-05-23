using System;
using Leopotam.EcsLite;
using UnityEngine;

namespace td.features.shards.ui
{
    [Serializable]
    public struct ShardUIDownEvent
    {
        public EcsPackedEntity packedEntity;
        public Vector2 position;
    }
}