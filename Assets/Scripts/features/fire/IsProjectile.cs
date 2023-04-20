using System;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UniLeo.Providers;
using td.common;

namespace td.features.fire
{
    [Serializable]
    [GenerateProvider]
    public struct IsProjectile
    {
        public EcsPackedEntity WhoFired;
    }
}