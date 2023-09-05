using System;
using System.Runtime.CompilerServices;
using td.features.state;
using td.features.state.interfaces;

namespace td.features.priceTimePopup
{

    [Serializable]
    public struct Event_PriceTimePopup_StateChanged : IStateChangedEvent
    {
        public bool visible;
        public bool price;
        public bool title;
        public bool isFine;
        public bool targetId;
        public bool time;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !visible && !price && !title && !isFine && !targetId && !time;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            visible = false;
            price = false;
            time = false;
            title = false;
            isFine = false;
            targetId = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            visible = true;
            price = true;
            time = true;
            title = true;
            isFine = true;
            targetId = true;
        }
    }
}