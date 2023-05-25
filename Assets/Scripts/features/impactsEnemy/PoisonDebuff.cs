using System;
using UnityEngine;

namespace td.features.impactsEnemy
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