using System;
using Leopotam.EcsLite;

namespace td.features.wave.bus
{
    [Serializable]
    public struct Wait_AllEnemiesAreOver : IEventUnique, IEventPersist
    {
        
    }
}