using System;
using td.common;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.components.commands
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
    }
}