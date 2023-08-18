using td.features.eventBus.types;
using Unity.Mathematics;

namespace td.features.level.bus
{
    public struct Event_CellBarrier_Clicked : IGlobalEvent
    {
        public int2 coords;
        public bool isLong;
    }
}