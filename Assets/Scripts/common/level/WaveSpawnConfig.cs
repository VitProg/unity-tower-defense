using System;
using td.utils;
using UnityEngine.Serialization;

namespace td.common.level
{
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