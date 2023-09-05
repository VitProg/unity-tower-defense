using System.Runtime.CompilerServices;
using td.features.state;
using td.features.state.interfaces;

namespace td.features.shard.shardStore
{
    public struct Event_ShardStore_StateChanged : IStateChangedEvent
    {
        public bool items;
        public bool visible;
        public bool x;
        public bool hoveredIndex;
        public bool level;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !items && !visible && !x && !hoveredIndex && !level;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            items = false;
            visible = false;
            x = false;
            hoveredIndex = false;
            level = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            items = true;
            visible = true;
            x = true;
            hoveredIndex = true;
            level = true;
        }
    }
}