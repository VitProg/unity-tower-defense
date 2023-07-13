using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.features.state;
using td.monoBehaviours;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.commands
{
    public class IdleRemoveGameObjectExecutor : IEcsRunSystem
    {
        [Inject] private State state;
        
        private readonly EcsFilterInject<Inc<IdleRemoveGameObjectCommand>, Exc<IsDestroyed>> entities = default;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (var entity in entities.Value)
            {
                ref var idle = ref entities.Pools.Inc1.Get(entity);

                idle.remainingTime -= Time.deltaTime * state.GameSpeed;

                if (idle.remainingTime > 0f) continue;
                
                world.DelComponent<IdleRemoveGameObjectCommand>(entity);
                world.GetComponent<RemoveGameObjectCommand>(entity);
            }
        }
    }
}