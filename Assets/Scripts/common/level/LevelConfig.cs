using System;
using td.features.shards;
using UnityEngine.Serialization;

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
        public int energy;
        public WaveConfig[] waves;
        public ShardTypes[] shardTypes;
        public Shard[] srartedShards;
    }
    
    // public enum LevelCellType
    // {
        // Square = 1,
        // Hex = 2
    // }
}