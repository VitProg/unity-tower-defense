using System;
using System.Runtime.CompilerServices;
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
        public Shard[] startedShards;
        public string[] availableShards;
        public ushort[] shardsCost;
    }
    
    // public enum LevelCellType
    // {
        // Square = 1,
        // Hex = 2
    // }
}