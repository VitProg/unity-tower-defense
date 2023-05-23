using System;
using System.Collections.Generic;
using td.common;
using UnityEngine.Serialization;

namespace td.features.enemies.components
{
    [Serializable]
    public struct EnemyPath
    {
        // public List<byte> steps;
        // public ushort count;
        public ushort index;
        public string spawnKey;
        public int pathNumber;
    }
}