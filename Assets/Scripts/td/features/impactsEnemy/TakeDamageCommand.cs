using System;
using Leopotam.EcsLite;

namespace td.features.impactsEnemy
{
    [Serializable]
    public struct TakeDamageCommand
    {
        public EcsPackedEntity TargetEntity;
        public float damage;
    }
}