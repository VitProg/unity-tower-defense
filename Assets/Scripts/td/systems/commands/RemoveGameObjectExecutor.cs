using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.components.commands;
using td.components.links;
using UnityEngine;

namespace td.systems.commands
{
    public class RemoveGameObjectExecutor: IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<RemoveGameObjectCommand, GameObjectLink>> entities = default;
        
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var entity in entities.Value)
            {
                ref var gameObjectLink = ref entities.Pools.Inc2.Get(entity);
                Object.Destroy(gameObjectLink.gameObject);
                
                world.DelEntity(entity);
            }
        }
    }
}