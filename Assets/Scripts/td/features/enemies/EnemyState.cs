using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.enemies
{
    [Serializable]
    public struct EnemyState
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

        public static EnemyState CreateFromSpawnCommand(SpawnEnemyOuterCommand command)
        {
            return new EnemyState()
            {
                enemyName = command.enemyName,
                spawner = command.spawner,
                speed = command.speed,
                angularSpeed = command.angularSpeed,
                health = command.health,
                damage = command.damage,
                scale = command.scale,
                offset = command.offset,
                money = command.money,
            };
        }
    }
}