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
        
        public void AutoMerge(ref PoisonDebuff result, PoisonDebuff def)
        {
            if (result.damage < def.damage)
            {
                result.damage = def.damage;
            }       
            
            if (result.damageInterval < def.damageInterval)
            {
                result.damageInterval = def.damageInterval;
                if (result.damageIntervalRemains > 0f)
                {
                    result.damageIntervalRemains = result.damageInterval - result.damageIntervalRemains;
                }
            }

            if (result.duration < def.duration)
            {
                result.duration = def.duration;
                if (result.timeRemains > 0f)
                {
                    result.timeRemains = result.duration;
                }
            }
        }
        
        public void Start()
        {
            timeRemains = duration;
            damageIntervalRemains = damageInterval;
            started = true;
        }
    }
}