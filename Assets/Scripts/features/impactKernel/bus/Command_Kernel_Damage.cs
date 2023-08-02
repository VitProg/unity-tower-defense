using System;
using Leopotam.EcsLite;

namespace td.features.impactKernel.bus
{
    [Serializable]
    public struct Command_Kernel_Damage : IEventGlobal
    {
        public float damage;
    }
}