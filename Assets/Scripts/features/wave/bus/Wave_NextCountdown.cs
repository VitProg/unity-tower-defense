using System;
using Leopotam.EcsLite;

namespace td.features.wave.bus
{
    [Serializable]
    public struct Wave_NextCountdown : IEventUnique, IEventPersist
    {
        public float countdown;
    }
}