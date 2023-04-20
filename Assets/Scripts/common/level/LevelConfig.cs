using System;

namespace td.common.level
{
    [Serializable]
    public struct LevelConfig
    {
        public uint levelNumber;
        // public LevelCellType cellType;
        public int delayBeforeFirstWave;
        public int delayBetweenWaves;
        public float lives;
        public int startedMoney;
        public WaveConfig[] waves;
    }
    
    // public enum LevelCellType
    // {
        // Square = 1,
        // Hex = 2
    // }
}