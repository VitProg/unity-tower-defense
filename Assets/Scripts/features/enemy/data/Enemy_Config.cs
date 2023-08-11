using System;
using UnityEngine;

namespace td.features.enemy.data
{
    [Serializable]
    public struct Enemy_Config
    {
        public string name;
        public string prefabPath;
        public GameObject prefab;
        public bool animated;
        public float animationSpeed;
        
        //
        public float baseSpeed;
        public float angularSpeed;
        public float baseHealth;
        public float baseDamage;
        
        //
        public EnemyConfigType[] types;
    }

    [Serializable]
    public struct EnemyConfigType
    {
        public float baseSpeed;
        public float angularSpeed;
        public float baseHealth;
        public float baseDamage;
        public bool animated;
        public float animationSpeed;
    }
}