using System;
using System.Runtime.CompilerServices;
using td.features.state;
using td.features.state.interfaces;

namespace td.features.infoPanel
{
    [Serializable]
    public struct Event_InfoPanel_StateChanged : IStateChangedEvent
    {
        public bool visible;
        public bool title;
        public bool priceTitle;
        public bool price;
        public bool timeTitle;
        public bool time;
        public bool before;
        public bool after;
        public bool shard;
        public bool enemy;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !visible && !title && !priceTitle && !price && !timeTitle && !time && !before && !after && !shard && !enemy;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            visible = false;
            title = false;
            priceTitle = false;
            price = false;
            timeTitle = false;
            time = false;
            before = false;
            after = false;
            shard = false;
            enemy = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            visible = true;
            title = true;
            priceTitle = true;
            price = true;
            timeTitle = true;
            time = true;
            before = true;
            after = true;
            shard = true;
            enemy = true;
        }
    }
}