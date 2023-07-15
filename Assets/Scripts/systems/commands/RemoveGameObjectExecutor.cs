using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.monoBehaviours;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.commands
{
    public class RemoveGameObjectExecutor : IEcsRunSystem
    {
        [Inject] private GameObjectPoolService poolServise;
        
        private readonly EcsFilterInject<Inc<RemoveGameObjectCommand, Ref<GameObject>>, Exc<IsDestroyed>> entities = default;

        public void Run(IEcsSystems systems)
        {
            // var world = systems.GetWorld();

            foreach (var entity in entities.Value)
            {
                var gameObject = entities.Pools.Inc2.Get(entity).reference;

                Remove(gameObject, entity);
            }
        }
        
        //todo ???
        public static void Remove(GameObject gameObject, int entity)
        {
            var poolServise = DI.Get<GameObjectPoolService>()!;
            var world = DI.GetWorld();
            
            var poolableObject = gameObject.GetComponent<PoolableObject>();

            if (poolableObject != null)
            {
                poolServise.Release(poolableObject);
                    
                // ToDo
                world.GetComponent<IsDestroyed>(entity);
                // world.DelEntity(entity);
            }
            else
            {
                Object.Destroy(gameObject);
                world.DelEntity(entity);
            }
        }
    }
}