using System;
using Leopotam.EcsLite;
using td.common;

namespace td.features.projectiles
{
    [Serializable]
    [GenerateProvider]
    public struct Projectile
    {
        public EcsPackedEntity WhoFired;
    }
}