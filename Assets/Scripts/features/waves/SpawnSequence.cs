using System;
using Leopotam.EcsLite;
using td.common.ecs;
using td.common.level;
using UnityEngine.Serialization;

namespace td.features.waves
{
    [Serializable]
    public struct SpawnSequence : IEcsAutoReset<SpawnSequence>, IEcsAutoMerge<SpawnSequence>
    {
        public WaveSpawnConfig config;
        public float delayBeforeCountdown;
        public float delayBetweenCountdown;
        public int enemyCounter;
        public bool started;
        public int lastSpawner;

        public void AutoReset(ref SpawnSequence c)
        {
            c = default;
            c.started = false;
            c.enemyCounter = 0;
            c.delayBetweenCountdown = c.config.delayBefore;
            c.delayBetweenCountdown = 0;
        }

        public void AutoMerge(ref SpawnSequence result, SpawnSequence def)
        {
            if (result.delayBeforeCountdown < Constants.ZeroFloat)
            {
                result.delayBeforeCountdown = def.delayBeforeCountdown;
            }
            if (result.delayBetweenCountdown < Constants.ZeroFloat)
            {
                result.delayBetweenCountdown = def.delayBetweenCountdown;
            }
            if (result.enemyCounter == 0)
            {
                result.enemyCounter = def.enemyCounter;
            }
        }
    }
}