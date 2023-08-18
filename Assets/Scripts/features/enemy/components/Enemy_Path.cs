using System;

namespace td.features.enemy.components
{
    [Serializable]
    public struct Enemy_Path
    {
        // public List<byte> steps;
        // public ushort count;
        public ushort index;
        public string spawnKey;
        public int pathNumber;
    }
}