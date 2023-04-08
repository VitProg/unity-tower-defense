using System;
using Leopotam.EcsLite;

namespace td.features.impactsEnemy
{
    [Serializable]
    public struct TakeDamageOuterCommand
    {
        public EcsPackedEntity TargetEntity;
        public float damage;
    }
}