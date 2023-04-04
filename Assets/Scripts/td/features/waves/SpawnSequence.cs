using System;
using Leopotam.EcsLite;
using td.common.ecs;
using td.common.level;

namespace td.features.waves
{
    [Serializable]
    public struct SpawnSequence : IEcsAutoReset<SpawnSequence>, IEcsAutoMerge<SpawnSequence>
    {
        public WaveSpawnConfig Config;
        public float DelayBeforeCountdown;
        public float DelayBetweenCountdown;
        public int EnemyCounter;
        public bool Started;

        public void AutoReset(ref SpawnSequence c)
        {
            c = default;
            c.Started = false;
            c.EnemyCounter = 0;
            c.DelayBetweenCountdown = c.Config.delayBefore;
            c.DelayBetweenCountdown = 0;
        }

        public void AutoMerge(ref SpawnSequence result, SpawnSequence def)
        {
            if (result.DelayBeforeCountdown < 0.0001f)
            {
                result.DelayBeforeCountdown = def.DelayBeforeCountdown;
            }
            if (result.DelayBetweenCountdown < 0.0001f)
            {
                result.DelayBetweenCountdown = def.DelayBetweenCountdown;
            }
            if (result.EnemyCounter == 0)
            {
                result.EnemyCounter = def.EnemyCounter;
            }
        }
    }
}