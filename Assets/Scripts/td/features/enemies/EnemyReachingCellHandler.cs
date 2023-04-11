using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common.cells;
using td.components;
using td.components.behaviors;
using td.components.commands;
using td.components.events;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies
{
    public class EnemyReachingCellHandler : IEcsRunSystem
    {
        [EcsInject] private LevelMap levelMap;
        [EcsWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<ReachingTargetEvent, Enemy, LinearMovementToTarget>> entities;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var enemy = ref entities.Pools.Inc2.Get(entity);
                ref var movementToTarget = ref entities.Pools.Inc3.Get(entity);
                ref var gameObjectLink = ref world.GetComponent<Ref<GameObject>>(entity);

                if (
                    !levelMap.TryGetCell<CellCanWalk>(movementToTarget.target, out var cell) ||
                    !levelMap.TryGetCell<CellCanWalk>(cell.NextCellCoordinates, out var nextCell)
                ) continue;
                
                if (cell.IsKernel)
                {
                    // send event
                    world.AddComponent<IsEnemyDead>(entity);
                    world.AddComponent<EnemyReachingKernelEvent>(entity);
                }
                else
                {
                    var rotation = EnemyUtils.LookToNextCell(cell.Coordinates, nextCell.Coordinates);

                    var transform = gameObjectLink.reference.transform;

                    if (transform.rotation != rotation)
                    {
                        var angularSpeed = EnemyUtils.GetAngularSpeed(world, entity);

                        if (angularSpeed < Constants.Enemy.SmoothRotationThreshold)
                        {
                            world.AddComponent(entity, new SmoothRotation()
                            {
                                From = transform.rotation,
                                To = rotation,
                                AngularSpeed = angularSpeed
                            });
                        }
                        else
                        {
                            transform.rotation = rotation;
                        }
                    }

                    movementToTarget.target = EnemyUtils.TargetPosition(
                        nextCell.Coordinates,
                        rotation,
                        enemy.offset
                    );
                    ;
                }
            }
        }
    }
}