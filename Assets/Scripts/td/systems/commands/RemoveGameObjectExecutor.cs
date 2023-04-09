using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using UnityEngine;

namespace td.systems.commands
{
    public class RemoveGameObjectExecutor: IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<RemoveGameObjectCommand, Ref<GameObject>>> entities = default;
        
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var entity in entities.Value)
            {
                ref var gameObjectLink = ref entities.Pools.Inc2.Get(entity);
                Object.Destroy(gameObjectLink.reference);
                
                world.DelEntity(entity);
            }
        }
    }
}