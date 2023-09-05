using System;
using Leopotam.EcsProto.QoL;
using td.utils.ecs;

namespace td.features.shard.systems
{
    public class Shard_UpdateAndInit_MB_System : ProtoIntervalableRunSystem
    {
        [DI] private Shard_MB_Service service;

        public override void IntervalRun(float deltaTime)
        {
            service.Update(deltaTime);
        }

        public Shard_UpdateAndInit_MB_System(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}