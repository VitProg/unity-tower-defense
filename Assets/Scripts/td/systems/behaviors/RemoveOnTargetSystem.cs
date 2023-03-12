using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.attributes;
using td.components.commands;

namespace td.systems.behaviors
{
    public class RemoveOnTargetSystem: IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Target, Position>> entities = default;
        
        private readonly EcsPoolInject<RemoveGameObjectCommand> removeGameObjectEventsPool = default;
        
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
                    removeGameObjectEventsPool.Value.Add(entity);
                }
            }
        }
    }
}