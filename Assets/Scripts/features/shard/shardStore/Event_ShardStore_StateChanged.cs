using System.Runtime.CompilerServices;
using td.features.state;

namespace td.features.shard.shardStore
{
    public struct Event_ShardStore_StateChanged : IStateChangedEvent
    {
        public bool items;
        public bool visible;
        public bool x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !items && !visible && !x;

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