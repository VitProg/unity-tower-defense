using System;
using Leopotam.EcsLite;

namespace td.features.impactsEnemy
{
    [Serializable]
    public struct TakeDamageOuter
    {
        public EcsPackedEntity targetEntity;
        public float damage;
        public DamageType type;
    }
}