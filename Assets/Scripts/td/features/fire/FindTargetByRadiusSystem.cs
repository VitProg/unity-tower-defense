using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.components.links;
using td.features.enemies;
using td.features.fire.components;
using td.services;
using td.utils;
using td.utils.ecs;

namespace td.features.fire
{
    public class FindTargetByRadiusSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<IsTower, GameObjectLink>> entities = default;
        private readonly EcsFilterInject<Inc<IsEnemy, GameObjectLink>> enemyEntities = default;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var entity in entities.Value)
            {
                var tower = entities.Pools.Inc1.Get(entity);
                var towerGameObject = entities.Pools.Inc2.Get(entity);

                var towerPosition = towerGameObject.gameObject.transform.position;

                // var sortedList = new SortedList<double, int>();
                var minDistance = double.MaxValue;
                var targetEntity = -1;

                foreach (var enemyEntity in enemyEntities.Value)
                {
                    var enemy = enemyEntities.Pools.Inc1.Get(enemyEntity);
                    var enemyGameObject = enemyEntities.Pools.Inc2.Get(enemyEntity);
                    var enemyPosition = enemyGameObject.gameObject.transform.position;

                    if (
                        Math.Abs(enemyPosition.x - towerPosition.x) > tower.radius ||
                        Math.Abs(enemyPosition.y - towerPosition.y) > tower.radius
                    )
                    {
                        continue;
                    }

                    if ((enemyPosition - towerPosition).sqrMagnitude < tower.radius * tower.radius)
                    {
                        var distanceToKernel = 0f; //Math.Sqrt(sqrDistance);
                        
                        //todo select method by tower settings
                        var enemyCoordinate = GridUtils.GetGridCoordinate(enemyPosition);
                        var cell = levelData.Value.GetCell(enemyCoordinate);
                        if (!cell.isKernel && enemy.distanceToKernel > 0)
                        {
                            distanceToKernel = enemy.distanceToKernel;
                        }

                        if (minDistance > distanceToKernel)
                        {
                            minDistance = distanceToKernel;
                            targetEntity = enemyEntity;
                        }
                    }
                }

                if (targetEntity >= 0)
                {
                    EntityUtils.DelComponent<FireTarget>(systems, entity);
                    EntityUtils.AddComponent(systems, entity, new FireTarget()
                    {
                        TargetEntity = world.PackEntity(targetEntity),
                    });
                }

                // sortedList.Clear();
            }
        }
    }
}