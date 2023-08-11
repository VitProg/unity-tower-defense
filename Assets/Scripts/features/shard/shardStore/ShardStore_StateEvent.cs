using System.Runtime.CompilerServices;

namespace td.features.shard.shardStore
{
    public struct ShardStore_StateEvent
    {
        public bool? items;
        public bool? visible;
        public bool? x;

        public bool IsEmpty => !items.HasValue && !visible.HasValue && !x.HasValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            items = false;
            visible = false;
            x = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            items = true;
            visible = true;
            x = true;
        }
    }
}