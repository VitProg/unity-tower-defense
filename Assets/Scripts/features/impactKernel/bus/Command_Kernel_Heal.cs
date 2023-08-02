using System;
using Leopotam.EcsLite;

namespace td.features.impactKernel.bus
{
    [Serializable]
    public struct Command_Kernel_Heal : IEventGlobal
    {
        public float heal;
    }
}