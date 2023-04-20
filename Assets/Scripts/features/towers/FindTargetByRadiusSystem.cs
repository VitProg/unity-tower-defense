using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.refs;
using td.features.enemies;
using td.features.fire;
using td.monoBehaviours;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.towers
{
    public class FindTargetByRadiusSystem : IEcsRunSystem
    {
        [EcsInject] private LevelMap levelMap;
        
        private readonly EcsFilterInject<Inc<Tower, Ref<GameObject>>> entities = default;
        private readonly EcsFilterInject<Inc<Enemy, Ref<GameObject>>> enemyEntities = default;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var entity in entities.Value)
            {
                var tower = entities.Pools.Inc1.Get(entity);
                var towerGameObject = entities.Pools.Inc2.Get(entity);

                var towerPosition = towerGameObject.reference.transform.position;

                // var sortedList = new SortedList<double, int>();
                var maxDistanceFromSpawn = 0f;
                var targetEntity = -1;

                foreach (var enemyEntity in enemyEntities.Value)
                {
                    var enemy = enemyEntities.Pools.Inc1.Get(enemyEntity);
                    var enemyGameObject = enemyEntities.Pools.Inc2.Get(enemyEntity);
                    var enemyPosition = enemyGameObject.reference.transform.position;

                    if (
                        Math.Abs(enemyPosition.x - towerPosition.x) > tower.radius ||
                        Math.Abs(enemyPosition.y - towerPosition.y) > tower.radius
                    )
                    {
                        continue;
                    }

                    if ((enemyPosition - towerPosition).sqrMagnitude < tower.radius * tower.radius)
                    {
                        var distanceFromSpawn = 999f; //Math.Sqrt(sqrDistance);
                        
                        //todo select method by tower settings
                        var enemyCoordinate = HexGridUtils.PositionToCell(enemyPosition);
                        var cell = levelMap.GetCell(enemyCoordinate, CellTypes.CanWalk);
                        if (cell && cell.isSpawn && enemy.distanceFromSpawn > 0)
                        {
                            distanceFromSpawn = enemy.distanceFromSpawn;
                        }

                        if (maxDistanceFromSpawn < distanceFromSpawn)
                        {
                            maxDistanceFromSpawn = distanceFromSpawn;
                            targetEntity = enemyEntity;
                        }
                    }
                }

                if (targetEntity >= 0)
                {
                    world.DelComponent<FireTarget>(entity);
                    world.AddComponent(entity, new FireTarget()
                    {
                        TargetEntity = world.PackEntity(targetEntity),
                    });
                }
            }
        }
    }
}