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
        public float baseSpeed;
        public float angularSpeed;
        public float baseHealth;
        public float baseDamage;
    }
}