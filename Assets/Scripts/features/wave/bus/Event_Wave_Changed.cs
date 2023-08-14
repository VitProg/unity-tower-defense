using System;
using td.features.eventBus.types;

namespace td.features.wave.bus
{
    [Serializable]
    public struct Event_Wave_Changed : IUniqueEvent
    {
        public int waveNumber;
    }
}