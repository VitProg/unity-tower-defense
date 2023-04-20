using System;
using td.utils;
using UnityEngine.Serialization;

namespace td.common.level
{
    [Serializable]
    public struct WaveSpawnConfig
    {
        public int spawner;
        public string[] enemies;
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

    public enum MethodOfSelectNextEnemy
    {
        Random = 1,
        Sequence = 2
    }
}