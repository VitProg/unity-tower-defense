using System;
using UnityEngine;

namespace td.features.enemies.components
{
    [Serializable]
    public struct SpawnEnemyOuterCommand
    {
        public string enemyName;
        public int enemyType;
        public int enemyVariant;
        public int spawner;
        public float speed;
        public float angularSpeed;
        public float health;
        public float damage;
        public float scale;
        public Vector2 offset;
        public int money;
    }
}