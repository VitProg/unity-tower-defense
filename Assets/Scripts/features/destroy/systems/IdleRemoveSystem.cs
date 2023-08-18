using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy.bus;
using td.features.eventBus;
using td.features.state;
using UnityEngine;

namespace td.features.destroy.systems
{
    public class IdleRemoveSystem : IProtoRunSystem
    {
        [DI] private Destroy_Service destroyService;
        [DI] private State state;
        [DI] private EventBus events;
        
        public void Run()
        {
            if (!events.global.Has<Command_Idle_Remove>()) return;

            var evPool = events.global.GetPool<Command_Idle_Remove>();
            var evIt = events.global.It<Command_Idle_Remove>();
            
            foreach (var evEntity in evIt)
            {
                ref var idle = ref evPool.Get(evEntity);
  
                idle.remainingTime -= Time.deltaTime * state.GetGameSpeed();

                if (idle.remainingTime > 0f) continue;

                events.global.Add<Command_Remove>().Entity = idle.Entity;
                events.global.Del(evEntity);
            }
        }
    }
}