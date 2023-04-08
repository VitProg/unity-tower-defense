using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.attributes;
using td.components.commands;
using td.utils.ecs;

namespace td.systems.behaviors
{
    public class RemoveOnTargetSystem: IEcsRunSystem
    {
        [EcsPool] private EcsPool<RemoveGameObjectCommand> removeGameObjectEventsPool;
        
        private readonly EcsFilterInject<Inc<Target, Position>> entities = default;


        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var entity in entities.Value)
            {
                var target = entities.Pools.Inc1.Get(entity);
                var position = entities.Pools.Inc2.Get(entity).position;

                var distance = (target.target - position).sqrMagnitude;

                if (distance <= target.gap * target.gap)
                {
                    removeGameObjectEventsPool.Add(entity);
                }
            }
        }
    }
}