using System;
using Leopotam.EcsLite;

namespace td.features.fire.components
{
    [Serializable]
    public struct FireTarget
    {
        public EcsPackedEntity TargetEntity;
    }
}