using System;
using Leopotam.EcsProto.QoL;

namespace td.features.projectile.components
{
    // todo move to Projectile
    [Serializable]
    public struct ProjectileTarget
    {
        public ProtoPackedEntityWithWorld targetEntity;
    }
}