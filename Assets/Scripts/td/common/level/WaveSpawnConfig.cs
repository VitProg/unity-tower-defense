using System;
using td.common.levelEvents;

namespace td.common.level
{
    [Serializable]
    public struct WaveSpawnConfig
    {
        public int spawner;
        public string[] enemies;
        public SelectEnemyTypeMethod selectMethod;
        public int quantity;
        public float speed;
        public float health;
        public float damage;
        public int delayBefore;
        public int delayBetween;
    }

    public enum MethodOfSelectNextEnemy
    {
        Random,
        Sequence
    }
}