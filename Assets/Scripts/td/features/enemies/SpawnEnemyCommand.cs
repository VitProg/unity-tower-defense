using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.enemies
{
    [Serializable]
    public struct SpawnEnemyCommand
    {
        public string enemyName;
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