using System;
using Leopotam.EcsLite;

namespace td.features.impactsEnemy
{
    [Serializable]
    public struct TakeDamageOuter
    {
        public EcsPackedEntity TargetEntity;
        public float damage;
    }
}