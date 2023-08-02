using System;
using System.Runtime.CompilerServices;

namespace td.features.infoPanel
{
    [Serializable]
    public struct InfoPanel_StateEvent
    {
        public bool? visible;
        public bool? title;
        public bool? costTitle;
        public bool? cost;
        public bool? before;
        public bool? after;
        public bool? shard;
        public bool? enemy;
        
        public bool IsEmpty => visible.HasValue && !title.HasValue && !costTitle.HasValue && !cost.HasValue && !before.HasValue && !after.HasValue && !shard.HasValue && !enemy.HasValue;

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