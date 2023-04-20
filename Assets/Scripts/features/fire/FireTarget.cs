using System;
using Leopotam.EcsLite;

namespace td.features.fire
{
    [Serializable]
    public struct FireTarget
    {
        public EcsPackedEntity TargetEntity;
    }
}