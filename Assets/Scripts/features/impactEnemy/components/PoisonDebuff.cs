using System;
using Leopotam.EcsLite;

namespace td.features.impactEnemy.components
{
    [Serializable]
    public struct PoisonDebuff
    {
        public float damage;
        public float damageInterval;
        public float duration;
        
        public float timeRemains;
        public float damageIntervalRemains;
        public bool started;
    }
}