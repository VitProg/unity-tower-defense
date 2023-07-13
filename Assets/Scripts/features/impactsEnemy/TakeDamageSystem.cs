using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.components.refs;
using td.features.enemies;
using td.features.enemies.components;
using td.features.enemies.mb;
using td.utils.ecs;
using UnityEngine;

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

                if (!takeDamage.targetEntity.Unpack(world, out var enemyEntity) ||
                    !world.HasComponent<Enemy>(enemyEntity) ||
                    !world.HasComponent<Ref<GameObject>>(enemyEntity) ||
                    world.HasComponent<IsEnemyDead>(enemyEntity)
                )
                {
                    continue;
                }

                ref var enemy = ref world.GetComponent<Enemy>(enemyEntity);
                var enemyGO = world.GetComponent<Ref<GameObject>>(enemyEntity).reference;
                
                // ToDo add effect for takeDamage.type !!!
                
                var enemyMb = enemyGO.GetComponent<EnemyMonoBehaviour>();

                enemy.health -= takeDamage.damage;
                enemyMb.hp.value = enemy.health;

                var p = enemy.health / enemy.startingHealth;

                enemyMb.hpLine.color = new Color(1, p, 0);

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