using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common.bus;
using td.features.state;
using UnityEngine;

namespace td.features._common.systems
{
    public class IdleRemoveSystem : IEcsRunSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsWorldInject world;
        
        public void Run(IEcsSystems systems)
        {
            if (!events.Value.Entity.Has<Command_Idle_Remove>()) return;

            var filter = events.Value.Entity.GetFilter<Command_Idle_Remove>();
            var pool = events.Value.Entity.GetPool<Command_Idle_Remove>();
            
            foreach (var idleEntity in filter)
            {
                ref var idle = ref pool.Get(idleEntity);

                idle.remainingTime -= Time.deltaTime * state.Value.GameSpeed;

                if (idle.remainingTime > 0f) continue;
                
                pool.GetWorld().DelEntity(idleEntity);
                
                common.Value.RemoveImmediately(idle.Entity);
            }
        }
    }
}