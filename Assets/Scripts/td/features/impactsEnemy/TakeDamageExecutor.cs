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
        private readonly EcsFilterInject<Inc<TakeDamageCommand>> eventEntities = Constants.Ecs.EventsWorldName;
        private readonly EcsWorldInject world = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                var takeDamage = eventEntities.Pools.Inc1.Get(eventEntity);

                if (!takeDamage.TargetEntity.Unpack(world.Value, out var enemyEntity) ||
                    !EntityUtils.HasComponent<SpawnEnemyCommand>(systems, enemyEntity) ||
                    EntityUtils.HasComponent<IsEnemyDead>(systems, enemyEntity)
                )
                {
                    continue;
                }

                ref var enemyStat = ref EntityUtils.GetComponent<SpawnEnemyCommand>(systems, enemyEntity);

                enemyStat.health -= takeDamage.damage;

                if (enemyStat.health < 0)
                {
                    EntityUtils.AddComponent<IsEnemyDead>(world.Value, enemyEntity);
                    EcsEventUtils.Send(systems, new EnemyDiedCommand()
                    {
                        EnemyEntity = world.Value.PackEntity(enemyEntity)
                    });
                }
            }
        }
    }
}