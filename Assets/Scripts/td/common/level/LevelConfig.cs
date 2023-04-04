using System;

namespace td.common.level
{
    [Serializable]
    public struct LevelConfig
    {
        public int levelNumber;
        public int delayBeforeFirstWave;
        public int delayBetweenWaves;
        public float lives;
        public int startedMoney;
        public WaveConfig[] waves;
    }
}