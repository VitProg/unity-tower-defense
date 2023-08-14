using System;
using System.Collections.Generic;
using Leopotam.EcsProto.QoL;
using td.features.destroy;
using td.features.enemy;
using td.features.movement;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile.lightning
{
    /**
     * Periodically updates the chain of enemies, looking for a new neighbor for the first, second, etc.
     */
    public class LightningNeighborsSystem : ProtoIntervalableRunSystem
    {
        [DI] private Lightning_Aspect lightningAspect;
        [DI] private Projectile_Aspect projectileAspect;
        [DI] private Enemy_Service enemyService; 
        [DI] private Movement_Service movementService;
        [DI] private Destroy_Service destroyService; 

        public override void IntervalRun(float deltaTime)
        {
            foreach (var lightningEntity in lightningAspect.it)
            {
                ref var lightning = ref lightningAspect.lightningPool.Get(lightningEntity);
                ref var lightningAttr = ref projectileAspect.lightningAttributePool.Get(lightningEntity);

                // берем первого живого врага в цепочки и от него строим новую
                ProtoPackedEntityWithWorld? firstPackedEntity = null;
                foreach (var chainPackedEntity in lightning.chainEntities)
                {
                    if (
                        !chainPackedEntity.Unpack(out _, out var chainEntity) ||
                        !enemyService.IsAlive(chainEntity)
                    ) continue;
                    firstPackedEntity = chainPackedEntity;
                    break;
                }

                // если больше нет ни одного живого врага в цепочке, то удаляем LightningLine
                if (
                    !firstPackedEntity.HasValue ||
                    !firstPackedEntity.Value.Unpack(out _, out var firstEntity)
                )
                {
                    destroyService.MarkAsRemoved(lightningEntity, projectileAspect.World());
                    continue;
                }
                
                var sqrChainRadius = Mathf.Pow(lightningAttr.chainReactionRadius, 2f);

                var chainOfEnemies = new List<int> { firstEntity };
                
                // ищем следующего ближайшего врага, из тех что отобрали выше, добавляем в список и повторяем поиск для нового.
                // todo
                var index = 0;
                while (true)
                {
                    var enemy = chainOfEnemies[index];

                    var position = movementService.GetTransform(enemy).position;

                    // ищем ближайшего соседа
                    var minSqrDistanse = float.MaxValue;
                    var nextEnemy = -1;
                    var enemiesInRadius = enemyService.FindNearestEnemies(position, sqrChainRadius);
                    
                    for (var idx = 0; idx < enemiesInRadius.Len(); idx++)
                    {
                        var potentialEnemy = enemiesInRadius.Get(idx);

                        // враг не должен уже находится в цепочке
                        if (chainOfEnemies.Contains(potentialEnemy)) continue;
                        
                        var potentialEnemyPosition = movementService.GetTransform(potentialEnemy).position;

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
                    if (index >= chainOfEnemies.Count || index >= lightningAttr.chainReaction)
                    {
                        break;
                    }
                }
                
                // обновляем даные по цепочке в компоненте
                lightning.length = chainOfEnemies.Count;
                for (var i = 0; i < Constants.WeaponEffects.MaxLightningChainReaction; i++)
                {
                    lightning.chainEntities[i] = i < lightning.length
                        ? enemyService.World().PackEntityWithWorld(chainOfEnemies[i])
                        : default;
                }
                
                // если в цепочке всего один враг, то удаляем эффект
                if (chainOfEnemies.Count < 2) {
                    destroyService.MarkAsRemoved(lightningEntity, projectileAspect.World());
                }
            }
        }

        public LightningNeighborsSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}