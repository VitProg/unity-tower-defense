using System;
using Leopotam.EcsLite;

namespace td.features.impactEnemy.components
{
    [Serializable]
    public struct SpeedDebuff
    {
        public float speedMultipler;
        public float duration;

        public float timeRemains;
        public bool started;
        public uint phase;
    }
}