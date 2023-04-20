using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.behaviors;
using td.components.commands;
using td.components.events;
using td.components.refs;
using td.monoBehaviours;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Random = UnityEngine.Random;

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

                var cell = levelMap.GetCell(movementToTarget.target);

                if (cell == null) continue;

                var nextCell = levelMap.GetCell(cell.GetRandomNextCoords(), CellTypes.CanWalk);

                if (cell.isKernel)
                {
                    // send event
                    world.AddComponent<IsEnemyDead>(entity);
                    world.AddComponent<EnemyReachingKernelEvent>(entity);
                }
                else if (nextCell != null)
                {
                    var rotation = EnemyUtils.LookToNextCell(cell, nextCell);

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

                    movementToTarget.from = movementToTarget.target;
                    movementToTarget.target = EnemyUtils.Position(
                        nextCell.Coords,
                        rotation,
                        enemy.offset
                    );

                    enemy.distanceFromSpawn += (movementToTarget.target - movementToTarget.from).magnitude;
                }
            }
        }
    }
}