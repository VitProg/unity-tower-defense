using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.features.enemies;
using td.features.enemies.components;
using td.utils.ecs;

namespace td.features.impactsEnemy
{
    public class TakeDamageSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<TakeDamageOuter>, Exc<IsDestroyed>> eventEntities = Constants.Worlds.Outer;
        [InjectWorld] private EcsWorld world;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                var takeDamage = eventEntities.Pools.Inc1.Get(eventEntity);

                if (!takeDamage.TargetEntity.Unpack(world, out var enemyEntity) ||
                    !world.HasComponent<Enemy>(enemyEntity) ||
                    world.HasComponent<IsEnemyDead>(enemyEntity)
                )
                {
                    continue;
                }

                ref var enemy = ref world.GetComponent<Enemy>(enemyEntity);

                enemy.health -= takeDamage.damage;

                if (enemy.health < 0)
                {
                    world.GetComponent<IsDisabled>(enemyEntity);
                    world.GetComponent<IsEnemyDead>(enemyEntity);
                    world.GetComponent<EnemyDiedCommand>(enemyEntity);
                }
            }
        }
    }
}