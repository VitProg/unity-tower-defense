using System;
using td.common;
using UnityEngine;

namespace td.features.enemies
{
    [Serializable]
    [GenerateProvider]
    public struct Enemy
    {
        public string enemyName;

        public float distanceToKernel;
        public float distanceFromSpawn;
        
        //GameObjectLink
        public Vector2 position;
        public Quaternion rotation;
        public float scale;
        public Vector2 offset;
        
        public int spawner;
        public float startingSpeed;
        public float startingHealth;
        public float angularSpeed;
        
        //EnemyState
        public float speed;
        public float health;
        public float damage;
        
        public int money;

        public void Setup(Vector2 position, Quaternion rotation, SpawnEnemyOuterCommand spawnCommand)
        {
            this.position = position;
            this.rotation = rotation;
            enemyName = spawnCommand.enemyName;
            spawner = spawnCommand.spawner;
            speed = spawnCommand.speed;
            startingSpeed = spawnCommand.speed;
            angularSpeed = spawnCommand.angularSpeed;
            health = spawnCommand.health;
            startingHealth = spawnCommand.health;
            damage = spawnCommand.damage;
            scale = spawnCommand.scale;
            offset = spawnCommand.offset;
            money = spawnCommand.money;
        }
    }
}