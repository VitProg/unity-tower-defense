using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.features.impactsKernel;
using td.services;
using td.states;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies
{
    public class EnemyReachingKernelEventHandle : IEcsRunSystem {
        [EcsInject] private LevelState levelState;
        [EcsWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<EnemyReachingKernelEvent, EnemyState>> entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var enemyState = ref entities.Pools.Inc2.Get(entity);
                
                //todo тут можно запустиить анимацию атаки на ядро, проподания врага, эфекты, вычитание жизней ядра и т.п.
                world.AddComponent<RemoveGameObjectCommand>(entity);
                
                systems.SendOuter(new KernalDamageOuterCommand()
                { 
                    damage = enemyState.damage
                });
                    
                // Debug.Log(">>> ENEMY IS REACHED KERNEL!!!!!");
            }
            
            if (entities.Value.GetEntitiesCount() > 0)
            {
                levelState.EnemiesCount = EnemyUtils.GetEnemiesCount(world);
            }
        }
    }
}