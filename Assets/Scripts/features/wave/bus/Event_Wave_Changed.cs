using System;
using Leopotam.EcsLite;

namespace td.features.wave.bus
{
    [Serializable]
    public struct Event_Wave_Changed : IEventUnique
    {
        public int waveNumber;
    }
}