using System;
using td.features.eventBus.types;

namespace td.features.level.bus
{
    [Serializable]
    public struct Command_LoadLevel: IUniqueEvent
    {
        public ushort levelNumber;
    }
}