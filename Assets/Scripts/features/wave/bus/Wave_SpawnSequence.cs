using System;
using Leopotam.EcsLite;
using td.features.level.data;

namespace td.features.wave.bus
{
    [Serializable]
    // public struct Wave_SpawnSequence : IEcsAutoReset<Wave_SpawnSequence>, IEcsAutoMerge<Wave_SpawnSequence>
    public struct Wave_SpawnSequence : IEventGlobal, IEventPersist
    {
        public LevelConfig.WaveSpawnConfig config;
        public float delayBeforeCountdown;
        public float delayBetweenCountdown;
        public int enemyCounter;
        public bool started;
        public int lastSpawner;

        /*public void AutoReset(ref Wave_SpawnSequence c)
        {
            c = default;
            c.started = false;
            c.enemyCounter = 0;
            c.delayBetweenCountdown = c.config.delayBefore;
            c.delayBetweenCountdown = 0;
        }

        public void AutoMerge(ref Wave_SpawnSequence result, Wave_SpawnSequence def)
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
        }*/
    }
}