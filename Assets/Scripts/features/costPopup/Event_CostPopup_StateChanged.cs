using System;
using System.Runtime.CompilerServices;
using td.features.state;

namespace td.features.costPopup
{

    [Serializable]
    public struct Event_CostPopup_StateChanged : IStateChangedEvent
    {
        internal bool visible;
        internal bool cost;
        internal bool title;
        internal bool isFine;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !visible && !cost && !title && !isFine;

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