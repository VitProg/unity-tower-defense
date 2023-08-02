using System;
using td.features.shard.components;
using UnityEngine.Serialization;

namespace td.features.level.data
{
    [Serializable]
    public struct LevelConfig
    {
        public ushort levelNumber;
        public byte delayBeforeFirstWave;
        public byte delayBetweenWaves;
        public float startedLives;
        public uint startedEnergy;
        public WaveConfig[] waves;
        public ShardsStore shardsStore;
        public ShardsCost shardsCost;
        public Shard[] startedShards;
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
    }


    // public enum LevelCellType
    // {
    // Square = 1,
    // Hex = 2
    // }
}