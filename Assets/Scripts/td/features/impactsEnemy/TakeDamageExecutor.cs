using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.features.enemies;
using td.features.impactsEnemy;
using td.utils.ecs;

namespace td.features.enemyImpacts
{
    public class TakeDamageExecutor : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<TakeDamageOuterCommand>> eventEntities = Constants.Worlds.Outer;
        [EcsWorld] private EcsWorld world;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                var takeDamage = eventEntities.Pools.Inc1.Get(eventEntity);

                if (!takeDamage.TargetEntity.Unpack(world, out var enemyEntity) ||
                    !world.HasComponent<EnemyState>(enemyEntity) ||
                    world.HasComponent<IsEnemyDead>(enemyEntity)
                )
                {
                    continue;
                }

                ref var enemyStat = ref world.GetComponent<EnemyState>(enemyEntity);

                enemyStat.health -= takeDamage.damage;

                if (enemyStat.health < 0)
                {
                    world.AddComponent<IsEnemyDead>(enemyEntity);
                    world.AddComponent<EnemyDiedCommand>(enemyEntity);
                }
            }
        }
    }
}