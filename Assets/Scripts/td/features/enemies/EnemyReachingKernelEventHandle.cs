using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.features.impactsKernel;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies
{
    public class EnemyReachingKernelEventHandle : IEcsRunSystem {
        private readonly EcsFilterInject<Inc<EnemyReachingKernelEvent>> eventEnteties = Constants.Ecs.EventsWorldName;
        private readonly EcsWorldInject world = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEnteties.Value)
            {
                ref var eventData = ref eventEnteties.Pools.Inc1.Get(eventEntity);

                if (!eventData.EnemyEntity.Unpack(world.Value, out var enemyEntity))
                {
                    continue;
                }
                
                //todo тут можно запустиить анимацию атаки на ядро, проподания врага, эфекты, вычитание жизней ядра и т.п.
                EntityUtils.AddComponent<RemoveGameObjectCommand>(systems, enemyEntity);

                var enemyStats = EntityUtils.GetComponent<SpawnEnemyCommand>(systems, enemyEntity);
                
                EcsEventUtils.Send(systems, new KernalDamageCommand()
                { 
                    damage = enemyStats.damage
                });
                    
                Debug.Log(">>> ENEMY IS REACHED KERNEL!!!!!");
            }

        }
    }
}