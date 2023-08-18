using System;
using System.Runtime.CompilerServices;
using td.features.state;

namespace td.features.pricePopup
{

    [Serializable]
    public struct Event_PricePopup_StateChanged : IStateChangedEvent
    {
        internal bool visible;
        internal bool price;
        internal bool title;
        internal bool isFine;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !visible && !price && !title && !isFine;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            visible = false;
            price = false;
            title = false;
            isFine = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            visible = true;
            price = true;
            title = true;
            isFine = true;
        }
    }
}