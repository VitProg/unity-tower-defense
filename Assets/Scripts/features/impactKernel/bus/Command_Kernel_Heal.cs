using System;
using td.features.eventBus.types;

namespace td.features.impactKernel.bus
{
    [Serializable]
    public struct Command_Kernel_Heal : IGlobalEvent
    {
        public float heal;
    }
}