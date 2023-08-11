using System;
using System.Runtime.CompilerServices;

namespace td.features.shard.shardCollection
{

    [Serializable]
    public struct ShardCollection_StateEvent
    {
        public bool? maxItems;
        public bool? items;
        public bool? hoveredItem;

        public bool IsEmpty => !items.HasValue && !maxItems.HasValue && !hoveredItem.HasValue;

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