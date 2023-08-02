using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features._common.flags;
using td.features.enemy;
using td.features.projectile.attributes;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile.lightning
{
    /**
     * Periodically updates the chain of enemies, looking for a new neighbor for the first, second, etc.
     */
    public class LightningLineNeighborsSystem : EcsIntervalableRunSystem
    {
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<Enemy_Pools> enemyPools;
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsWorldInject world;

        private readonly EcsFilterInject<Inc<LightningLine, LightningAttribute, Ref<GameObject>>, Exc<IsDisabled, IsDestroyed>> entities = default;
        
        public override void IntervalRun(IEcsSystems systems, float deltaTime)
        {
            foreach (var lightningLineEntity in entities.Value)
            {
                ref var lightningLine = ref entities.Pools.Inc1.Get(lightningLineEntity);
                ref var lightning = ref entities.Pools.Inc2.Get(lightningLineEntity);

                // lightningLine.findNeighborsTimeRemains -= Time.deltaTime * state.Value.GameSpeed;

                // if (lightningLine.findNeighborsTimeRemains > 0) continue;

                // lightningLine.findNeighborsTimeRemains = Constants.WeaponEffects.LightningFindNeighborsTimeRemains;

                // берем первого живого врага в цепочки и от него строим новую
                EcsPackedEntity? firstPackedEntity = null;
                foreach (var chainPackedEntity in lightningLine.chainEntities)
                {
                    if (
                        !chainPackedEntity.Unpack(world.Value, out var chainEntity) ||
                        !enemyService.Value.IsAlive(chainEntity)
                    ) continue;
                    firstPackedEntity = chainPackedEntity;
                    break;
                }

                // если больше нет ни одного живого врага в цепочке, то удаляем LightningLine
                if (
                    !firstPackedEntity.HasValue ||
                    !firstPackedEntity.Value.Unpack(world.Value, out var firstEntity)
                )
                {
                    common.Value.SafeDelete(lightningLineEntity);
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

                    var position = common.Value.GetGOPosition(enemy);

                    // ищем ближайшего соседа
                    var minSqrDistanse = float.MaxValue;
                    var nextEnemy = -1;
                    foreach (var potentialEnemy in enemyPools.Value.livingEnemiesFilter.Value)
                    {
                        // враг не должен уже находится в цепочке
                        if (chainOfEnemies.Contains(potentialEnemy)) continue;
                        
                        var potentialEnemyPosition = common.Value.GetGOPosition(potentialEnemy);

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
                        ? world.Value.PackEntity(chainOfEnemies[i])
                        : default;
                }
                
                // если в цепочке всего один враг, то удаляем эффект
                if (chainOfEnemies.Count < 2) {
                    common.Value.SafeDelete(lightningLineEntity);
                }
            }
        }

        public LightningLineNeighborsSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}