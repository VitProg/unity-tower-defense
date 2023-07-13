using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.features.enemies.components;
using td.features.projectiles.attributes;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles.lightning
{
    /**
     * Periodically updates the chain of enemies, looking for a new neighbor for the first, second, etc.
     */
    public class LightningLineNeighborsSystem : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;

        private readonly EcsFilterInject<Inc<LightningLine, LightningAttribute, Ref<GameObject>>, Exc<IsDisabled, IsDestroyed>> entities = default;

        private readonly EcsFilterInject<Inc<Enemy, Ref<GameObject>>, Exc<IsDisabled, IsDestroyed>> enemyEntities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var lightningLineEntity in entities.Value)
            {
                ref var lightningLine = ref entities.Pools.Inc1.Get(lightningLineEntity);
                ref var lightning = ref entities.Pools.Inc2.Get(lightningLineEntity);

                lightningLine.findNeighborsTimeRemains -= Time.deltaTime;

                if (lightningLine.findNeighborsTimeRemains > 0) continue;

                lightningLine.findNeighborsTimeRemains = Constants.WeaponEffects.LightningFindNeighborsTimeRemains;

                // берем первого живого врага в цепочки и от него строим новую
                EcsPackedEntity? firstPackedEntity = null;
                foreach (var chainEntity in lightningLine.chainEntities)
                {
                    if (!IsValidEnemy(chainEntity)) continue;
                    firstPackedEntity = chainEntity;
                    break;
                }

                // если больше нет ни одного живого врага в цепочке, то удаляем LightningLine
                if (
                    !firstPackedEntity.HasValue ||
                    !firstPackedEntity.Value.Unpack(world, out var firstEntity)
                )
                {
                    world.GetComponent<IsDisabled>(lightningLineEntity);
                    world.GetComponent<RemoveGameObjectCommand>(lightningLineEntity);
                    continue;
                }
                
                var sqrChainRadius = Mathf.Pow(lightning.chainReactionRadius, 2f);

                var chainOfEnemies = new List<int> { firstEntity };
                
                // ищем следующего ближайшего врага, из тех что отобрали выше, добавляем в список и повторяем поиск для нового.
                // todo
                var index = 0;
                while (true)
                {
                    var enemy = chainOfEnemies[index];

                    var position = (Vector2)enemyEntities.Pools.Inc2.Get(enemy).reference.transform.position;

                    // ищем ближайшего соседа
                    var minSqrDistanse = float.MaxValue;
                    var nextEnemy = -1;
                    foreach (var potentialEnemy in enemyEntities.Value)
                    {
                        // враг не должен уже находится в цепочке
                        if (chainOfEnemies.Contains(potentialEnemy)) continue;
                        
                        var potentialEnemyPosition = (Vector2)enemyEntities.Pools.Inc2.Get(potentialEnemy).reference.transform.position;

                        if (
                            Math.Abs(potentialEnemyPosition.x - position.x) > sqrChainRadius ||
                            Math.Abs(potentialEnemyPosition.y - position.y) > sqrChainRadius
                        )
                        {
                            continue;
                        }
                        
                        var sqrDistanse = (potentialEnemyPosition - position).sqrMagnitude;

                        if (sqrDistanse < sqrChainRadius && sqrDistanse < minSqrDistanse)
                        {
                            minSqrDistanse = sqrDistanse;
                            nextEnemy = potentialEnemy;
                        }
                    }

                    // если нашли, добавляем в цепочку и повторяем поиск для только что найденного
                    if (nextEnemy > -1)
                    {
                        chainOfEnemies.Add(nextEnemy);
                    }

                    index++;
                    if (index >= chainOfEnemies.Count || index >= lightning.chainReaction)
                    {
                        break;
                    }
                }
                
                // обновляем даные по цепочке в компоненте
                lightningLine.length = chainOfEnemies.Count;
                for (var i = 0; i < Constants.WeaponEffects.MaxLightningChainReaction; i++)
                {
                    lightningLine.chainEntities[i] = i < lightningLine.length
                        ? world.PackEntity(chainOfEnemies[i])
                        : default;
                }
                
                // если в цепочке всего один враг, то удаляем эффект
                if (chainOfEnemies.Count < 2) {
                    world.GetComponent<IsDisabled>(lightningLineEntity);
                    world.GetComponent<RemoveGameObjectCommand>(lightningLineEntity);
                }
            }
        }

        private bool IsValidEnemy(EcsPackedEntity enemyPackedEntity) =>
            enemyPackedEntity.Unpack(world, out var enemyEntity) &&
            IsValidEnemy(enemyEntity);
        
        private bool IsValidEnemy(int enemyEntity) =>
            world.HasComponent<Enemy>(enemyEntity) &&
            !world.HasComponent<IsDisabled>(enemyEntity) &&
            !world.HasComponent<IsDestroyed>(enemyEntity);
    }
}