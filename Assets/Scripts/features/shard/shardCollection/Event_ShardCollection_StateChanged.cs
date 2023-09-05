using System;
using System.Runtime.CompilerServices;
using td.features.state;
using td.features.state.interfaces;
using UnityEngine.Serialization;

namespace td.features.shard.shardCollection
{

    [Serializable]
    public struct Event_ShardCollection_StateChanged : IStateChangedEvent
    {
        public bool items;
        public bool hoveredIndex;
        public bool draggableShard;
        public bool operation;
        public bool operationTarget;
        public bool maxShards;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !items && !hoveredIndex && !draggableShard && !operation && !operationTarget && !maxShards;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            items = false;
            hoveredIndex = false;
            draggableShard = false;
            operation = false;
            operationTarget = false;
            maxShards = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            items = true;
            hoveredIndex = true;
            draggableShard = true;
            operation = true;
            operationTarget = true;
            maxShards = true;
        }
    }
}