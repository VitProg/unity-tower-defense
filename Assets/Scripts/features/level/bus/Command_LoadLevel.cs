using System;
using Leopotam.EcsLite;

namespace td.features.level.bus
{
    [Serializable]
    public struct Command_LoadLevel: IEventUnique
    {
        public ushort levelNumber;
    }
}