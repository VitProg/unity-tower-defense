using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.flags;
using td.features.enemies.components;
using td.features.impactsKernel;
using td.features.state;
using td.utils.ecs;

namespace td.features.enemies.systems
{
    public class EnemyReachingKernelEventHandle : IEcsRunSystem {
        [Inject] private State state;
        [InjectWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<EnemyReachingKernelEvent, Enemy>, Exc<IsDestroyed>> entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var enemyEntity in entities.Value)
            {
                ref var enemy = ref entities.Pools.Inc2.Get(enemyEntity);
                
                //todo тут можно запустиить анимацию атаки на ядро, проподания врага, эфекты, вычитание жизней ядра и т.п.
                world.GetComponent<IsDisabled>(enemyEntity);
                world.GetComponent<RemoveGameObjectCommand>(enemyEntity);

                systems.Outer<KernalDamageOuterCommand>().damage = enemy.damage;
                    
                // Debug.Log(">>> ENEMY IS REACHED KERNEL!!!!!");
            }
            
            if (entities.Value.GetEntitiesCount() > 0)
            {
                state.EnemiesCount = EnemyUtils.GetEnemiesCount(world);
            }
        }
    }
}