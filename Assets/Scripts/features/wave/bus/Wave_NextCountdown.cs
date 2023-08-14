using System;
using td.features.eventBus.types;

namespace td.features.wave.bus
{
    [Serializable]
    public struct Wave_NextCountdown : IUniqueEvent, IPersistEvent
    {
        public float countdown;
    }
}