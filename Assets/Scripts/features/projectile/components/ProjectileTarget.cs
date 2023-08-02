using System;
using Leopotam.EcsLite;

namespace td.features.projectile.components
{
    // todo move to Projectile
    [Serializable]
    public struct ProjectileTarget
    {
        public EcsPackedEntity targetEntity;
    }
}