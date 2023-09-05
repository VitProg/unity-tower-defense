using System;
using System.Runtime.CompilerServices;
using UnityEngine.Serialization;

namespace td.features._common.data
{
    [Serializable]
    public struct LevelConfig
    {
        public int levelNumber;
        
        public byte delayBeforeFirstWave;
        public byte delayBetweenWaves;
        
        public float lives;
        public uint energy;
        
        public float maxLives;
        public uint maxEnergy;
        
        public WaveConfig[] waves;
        public ShardsStore shardsStore;
        [FormerlySerializedAs("shardsCost")] public ShardsCost shardsPrice;
        public StartedShard[] startedShards;
        public byte maxShards;

        [Serializable]
        public struct ShardsCost
        {
            public uint red;
            public uint green;
            public uint blue;
            public uint aquamarine;
            public uint yellow;
            public uint orange;
            public uint pink;
            public uint violet;
        }

        [Serializable]
        public struct StartedShard
        {
            public byte red;
            public byte green;
            public byte blue;
            public byte aquamarine;
            public byte yellow;
            public byte orange;
            public byte pink;
            public byte violet;
            
            public override string ToString() => $"{red}-{green}-{blue}-{aquamarine}-{yellow}-{orange}-{pink}-{violet}";
        }

        [Serializable]
        public struct ShardsStore
        {
            public bool red;
            public bool green;
            public bool blue;
            public bool aquamarine;
            public bool yellow;
            public bool orange;
            public bool pink;
            public bool violet;
        }

        [Serializable]
        public struct WaveConfig
        {
            public WaveSpawnConfig[] spawns;
        }

        [Serializable]
        public struct WaveSpawnConfig
        {
            public int spawner;
            public WaveSpawnConfigEnemy[] enemies;
            public MethodOfSelectNextEnemy selectMethod;
            public int quantity;
            public float speed;
            public float health;
            public float damage;
            public float delayBefore;
            public float delayBetween;
            public float[] scale;
            public float[] offset;
        }

        [Serializable]
        public struct WaveSpawnConfigEnemy
        {
            public string name;
            public int type;
            public int variant;
        }

        public enum MethodOfSelectNextEnemy
        {
            Random = 0,
            Sequence = 1
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => levelNumber = 0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => levelNumber == 0;
        
    }


    // public enum LevelCellType
    // {
    // Square = 1,
    // Hex = 2
    // }
}