using System;
using Leopotam.EcsLite;
using td.utils.ecs;

namespace td.features.shard.systems
{
    public class MB_ShardUpdateAndInit_System : EcsIntervalableRunSystem, IEcsInitSystem
    {
        private readonly EcsInject<MB_Shard_Service> service;

        public override void IntervalRun(IEcsSystems systems, float dt)
        {
            service.Value.Update(dt);
        }

        public void Init(IEcsSystems systems)
        {
            service.Value.Init();
        }

        public MB_ShardUpdateAndInit_System(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}