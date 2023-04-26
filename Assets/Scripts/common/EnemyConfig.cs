using System;
using UnityEngine;

namespace td.common
{
    [Serializable]
    public struct EnemyConfig
    {
        public string name;
        public string prefabPath;
        public GameObject prefab;
        public bool animated;
        
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
    }
}