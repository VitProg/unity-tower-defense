using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.behaviors;
using td.components.commands;
using td.components.events;
using td.components.flags;
using td.components.refs;
using td.features.enemies.components;
using td.features.enemies.mb;
using td.monoBehaviours;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace td.features.enemies.systems
{
    public class EnemyReachingCellHandler : IEcsRunSystem
    {
        [Inject] private LevelMap levelMap;
        [Inject] private EnemyPathService enemyPathService;
        [InjectWorld] private EcsWorld world;

        private readonly EcsFilterInject<
            Inc<ReachingTargetEvent, Enemy, LinearMovementToTarget, Ref<GameObject>, EnemyPath>,
            Exc<IsDestroyed, IsEnemyDead>
        > entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var enemyEntity in entities.Value)
            {
                ref var enemy = ref entities.Pools.Inc2.Get(enemyEntity);
                ref var movementToTarget = ref entities.Pools.Inc3.Get(enemyEntity);
                ref var gameObjectLink = ref entities.Pools.Inc4.Get(enemyEntity);
                ref var enemyPath = ref entities.Pools.Inc5.Get(enemyEntity);

                var currentCell = levelMap.GetCell(movementToTarget.target);
                
                if (currentCell == null) continue;
                
                if (currentCell.isKernel)
                {
                    // send event
                    world.GetComponent<IsEnemyDead>(enemyEntity);
                    world.GetComponent<EnemyReachingKernelEvent>(enemyEntity);
                    
                    continue;
                }
               
                
                var path = enemyPathService.GetPath(enemyEntity);
                
                enemyPath.index++;
                if (enemyPath.index >= path.Count) continue;
                
                var step = path[enemyPath.index];
                
                
                // var nextCell = levelMap.GetCell(HexGridUtils.GetNeighborsCoords(currentCell.Coords, (HexDirections)step));
                var nextCell = levelMap.GetCell(step);
                
                // if (nextCell == null) continue;

                // var nextCell = levelMap.GetCell(currentCell.GetRandomNextCoords(), CellTypes.CanWalk);

                if (nextCell != null)
                {
                    var rotation = EnemyUtils.LookToNextCell(currentCell, nextCell);

                    var enemyMb = gameObjectLink.reference.GetComponent<EnemyMonoBehaviour>();

                    var transform = enemyMb.body.transform;

                    if (transform.rotation != rotation)
                    {
                        var angularSpeed = EnemyUtils.GetAngularSpeed(world, enemyEntity);

                        if (angularSpeed < Constants.Enemy.SmoothRotationThreshold)
                        {
                            ref var smoothRotation = ref world.GetComponent<SmoothRotation>(enemyEntity);
                            smoothRotation.from = transform.rotation;
                            smoothRotation.to = rotation;
                            smoothRotation.angularSpeed = angularSpeed;
                            smoothRotation.targetBody = enemyMb.body;
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