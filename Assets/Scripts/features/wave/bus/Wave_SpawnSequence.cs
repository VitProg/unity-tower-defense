using System;
using td.features._common.data;
using td.features.eventBus.types;

namespace td.features.wave.bus
{
    [Serializable]
    public struct Wave_SpawnSequence : IGlobalEvent, IPersistEvent
    {
        public LevelConfig.WaveSpawnConfig config;
        public float delayBeforeCountdown;
        public float delayBetweenCountdown;
        public int enemyCounter;
        public bool started;
        public int lastSpawner;
    }
}