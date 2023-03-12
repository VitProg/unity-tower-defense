using System;

namespace td.common.level
{
    [Serializable]
    public struct LevelConfig
    {
        public int levelNumber;
        public int delayBeforeFirstWave;
        public int delayBetweenWaves;
        public WaveConfig[] waves;
    }
}