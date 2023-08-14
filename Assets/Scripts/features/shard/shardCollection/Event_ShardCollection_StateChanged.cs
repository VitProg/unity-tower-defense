using System;
using System.Runtime.CompilerServices;
using td.features.state;

namespace td.features.shard.shardCollection
{

    [Serializable]
    public struct Event_ShardCollection_StateChanged : IStateChangedEvent
    {
        public bool maxItems;
        public bool items;
        public bool hoveredItem;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !maxItems && !items && !hoveredItem;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            maxItems = false;
            items = false;
            hoveredItem = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            maxItems = true;
            items = true;
            hoveredItem = true;
        }
    }
}