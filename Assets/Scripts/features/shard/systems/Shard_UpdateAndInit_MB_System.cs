using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level.bus;
using td.utils.ecs;

namespace td.features.shard.systems
{
    public class Shard_UpdateAndInit_MB_System : ProtoIntervalableRunSystem, IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private Shard_MB_Service service;
        [DI] private EventBus events;

        public override void IntervalRun(float deltaTime)
        {
            service.Update(deltaTime);
        }

        public Shard_UpdateAndInit_MB_System(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }

        public void Init(IProtoSystems systems) {
            events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
        }

        public void Destroy() {
            events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
        }

        private void OnLevelFinished(ref Event_LevelFinished ev) {
            service.Clear();
        }
    }
}