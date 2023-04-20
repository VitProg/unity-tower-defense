using System;
using td.common.ecs;


namespace td.features.impactsEnemy
{
    [Serializable]
    public struct SpeedDebuff : IEcsAutoMerge<SpeedDebuff>
    {
        public float speedMultipler;
        public float duration;

        public float timeRemains;
        public bool started;
        public uint phase;

        public void AutoMerge(ref SpeedDebuff result, SpeedDebuff def)
        {
            if (result.speedMultipler < def.speedMultipler)
            {
                result.speedMultipler = def.speedMultipler;
            }

            if (result.duration < def.duration)
            {
                result.duration = def.duration;
                if (result.timeRemains > 0f)
                {
                    result.timeRemains = result.duration;
                }
            }
        }

        public void Start()
        {
            timeRemains = duration;
            started = true;
        }
    }
}