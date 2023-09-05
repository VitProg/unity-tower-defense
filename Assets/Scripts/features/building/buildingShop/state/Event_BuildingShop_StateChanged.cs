using System.Runtime.CompilerServices;
using td.features.state;
using td.features.state.interfaces;

namespace td.features.building.buildingShop.state
{
    public struct Event_BuildingShop_StateChanged : IStateChangedEvent
    {
        public bool items;
        public int changedItem;
        public bool visible;
        public bool position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !items && !visible && !position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            items = false;
            visible = false;
            position = false;
            changedItem = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            items = true;
            visible = true;
            position = true;
            changedItem = -1;
        }
    }
}