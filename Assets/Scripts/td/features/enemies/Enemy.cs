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
        
        //GameObjectLink
        public Vector2 position;
        public Quaternion rotation;
        public float scale;
        public Vector2 offset;
        
        public int spawner;
        public float speed;
        public float angularSpeed;
        
        //EnemyState
        public float health;
        public float damage;
        
        public int money;
    }
}