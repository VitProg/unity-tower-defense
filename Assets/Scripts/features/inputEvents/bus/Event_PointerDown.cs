using td.features.eventBus.types;

namespace td.features.inputEvents.bus
{
    public struct Event_PointerDown : IGlobalEvent
    {
        public float x;
        public float y;
        public byte mouseButton;
        public bool isTouch;

        public override string ToString() => $"Event_PointerDown: x:{x} y:{y} mb:{mouseButton} t:{isTouch}";
    }
}