using System;
using td.common;

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
    }
}