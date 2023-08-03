using System;
using Leopotam.EcsLite;
using td.features._common;

namespace td.features.impactEnemy.components
{
    [Serializable]
    public struct TakeDamage : IEventEntity
    {
        public EcsPackedEntityWithWorld Entity { get; set; }
        // public EcsPackedEntity targetEntity { get; set; }
        public float damage;
        public DamageType type;
    }
}