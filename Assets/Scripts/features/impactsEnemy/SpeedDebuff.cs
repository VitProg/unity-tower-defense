using System;
using td.common.ecs;


namespace td.features.impactsEnemy
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