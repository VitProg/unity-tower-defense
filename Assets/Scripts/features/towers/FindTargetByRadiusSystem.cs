using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.features.enemies.components;
using td.features.projectiles;
using td.features.shards;
using td.monoBehaviours;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.towers
{
    public class FindTargetByRadiusSystem : IEcsRunSystem
    {
        [Inject] private LevelMap levelMap;
        [Inject] private ShardCalculator shardCalculator;
        
        private readonly EcsFilterInject<Inc<Tower, Ref<GameObject>>, Exc<IsDisabled, RemoveGameObjectCommand, IsDestroyed>> towerEntities = default;
        private readonly EcsFilterInject<Inc<Enemy, Ref<GameObject>>, Exc<IsDisabled, RemoveGameObjectCommand, IsDestroyed>> enemyEntities = default;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var towerEntity in towerEntities.Value)
            {
                var tower = towerEntities.Pools.Inc1.Get(towerEntity);
                var towerGameObject = towerEntities.Pools.Inc2.Get(towerEntity);

                var towerPosition = towerGameObject.reference.transform.position;

                var radius = tower.radius;
                
                if (ShardUtils.HasShardInTower(world, towerEntity))
                {
                    ref var shard = ref ShardUtils.GetShardInTower(world, towerEntity);
                    // yellow - увеличивает радиус стрельбы
                    radius = shardCalculator.GetTowerRadius(ref shard);
                }
                
                var towerSqrRadius = radius * radius;
                
                // var maxDistanceFromSpawn = 0f;
                var minDistanceToKernel = float.MaxValue;
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

                    if ((enemyPosition - towerPosition).sqrMagnitude < towerSqrRadius)
                    {
                        if (minDistanceToKernel > enemy.distanceToKernel)
                        {
                            minDistanceToKernel = enemy.distanceToKernel;
                            targetEntity = enemyEntity;
                        }
                        // var distanceFromSpawn = float.MaxValue; 
                        //
                        // //todo select method by tower settings
                        // var enemyCoordinate = HexGridUtils.PositionToCell(enemyPosition);
                        // var cell = levelMap.GetCell(enemyCoordinate, CellTypes.CanWalk);
                        // if (cell && cell.isSpawn && enemy.distanceFromSpawn > 0)
                        // {
                        //     distanceFromSpawn = enemy.distanceFromSpawn;
                        // }
                        //
                        // if (maxDistanceFromSpawn < distanceFromSpawn)
                        // {
                        //     maxDistanceFromSpawn = distanceFromSpawn;
                        //     targetEntity = enemyEntity;
                        // }
                    }
                }

                if (targetEntity >= 0)
                {
                    world.GetComponent<ProjectileTarget>(towerEntity).targetEntity = world.PackEntity(targetEntity);
                }
            }
        }
    }
}