using System;
using System.Runtime.CompilerServices;
using td.features.state;

namespace td.features.infoPanel
{
    [Serializable]
    public struct Event_InfoPanel_StateChanged : IStateChangedEvent
    {
        public bool visible;
        public bool title;
        public bool costTitle;
        public bool cost;
        public bool before;
        public bool after;
        public bool shard;
        public bool enemy;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !visible || (!title && !costTitle && !cost && !before && !after && !shard && !enemy);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            visible = false;
            title = false;
            costTitle = false;
            cost = false;
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
            costTitle = true;
            cost = true;
            before = true;
            after = true;
            shard = true;
            enemy = true;
        }
    }
}