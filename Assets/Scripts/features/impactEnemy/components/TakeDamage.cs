using System;
using Leopotam.EcsLite;

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