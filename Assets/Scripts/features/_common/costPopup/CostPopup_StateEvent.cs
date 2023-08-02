using System;
using System.Runtime.CompilerServices;

namespace td.features._common.costPopup
{

    [Serializable]
    public struct CostPopup_StateEvent
    {
        internal bool? visible;
        internal bool? cost;
        internal bool? title;
        internal bool? isFine;

        public bool IsEmpty => !visible.HasValue && !title.HasValue && !cost.HasValue && !isFine.HasValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            visible = false;
            cost = false;
            title = false;
            isFine = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            visible = true;
            cost = true;
            title = true;
            isFine = true;
        }
    }
}