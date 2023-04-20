using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.enemies;
using td.utils.ecs;

namespace td.features.impactsEnemy
{
    public class TakeDamageSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<TakeDamageOuter>> eventEntities = Constants.Worlds.Outer;
        [EcsWorld] private EcsWorld world;

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
                    world.AddComponent<IsEnemyDead>(enemyEntity);
                    world.AddComponent<EnemyDiedCommand>(enemyEntity);
                }
            }
        }
    }
}