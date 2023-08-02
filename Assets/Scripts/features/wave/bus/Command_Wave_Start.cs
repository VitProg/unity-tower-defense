using System;
using Leopotam.EcsLite;

namespace td.features.wave.bus
{
    [Serializable]
    public struct Command_Wave_Start : IEventUnique
    {
        public int waveNumber;
    }
}