using td.features.eventBus.types;

namespace td.features.inputEvents.bus
{
    public struct Event_PointerUp : IGlobalEvent
    {
        public float x;
        public float y;
        public byte mouseButton;
    }
}