using System;
using Leopotam.EcsLite;
using UnityEngine.Serialization;

namespace td.features.impactEnemy.components
{
    [Serializable]
    public struct PoisonDebuff
    {
        public float damage;
        public float duration;
        
        public float timeRemains;
        public float intervalRemains;
        public bool started;
    }
}