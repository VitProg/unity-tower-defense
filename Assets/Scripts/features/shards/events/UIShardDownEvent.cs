using System;
using Leopotam.EcsLite;
using UnityEngine;

namespace td.features.shards.events
{
    [Serializable]
    public struct UIShardDownEvent
    {
        public EcsPackedEntity packedEntity;
        public Vector2 position;
    }
}