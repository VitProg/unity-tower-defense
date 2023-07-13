using System;
using Leopotam.EcsLite;
using td.common;

namespace td.features.projectiles
{
    [Serializable]
    public struct Projectile
    {
        public EcsPackedEntity whoFired;
    }
}