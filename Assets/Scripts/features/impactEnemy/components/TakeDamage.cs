using System;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.eventBus.types;

namespace td.features.impactEnemy.components
{
    [Serializable]
    public struct TakeDamage : IGlobalEvent
    {
        public ProtoPackedEntityWithWorld entity;
        public float damage;
        public DamageType type;
    }
}