using System;
using Leopotam.EcsLite;

namespace td.features.projectiles
{
    [Serializable]
    public struct ProjectileTarget
    {
        public EcsPackedEntity TargetEntity;
    }
}