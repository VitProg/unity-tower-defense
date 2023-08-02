using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.features._common;
using td.features._common.components;
using td.features._common.flags;
using td.features.enemy.bus;
using td.features.enemy.components;
using td.features.level;
using td.utils;
using UnityEngine;

namespace td.features.enemy.systems
{
    public class EnemyReachingCellSystem : IEcsRunSystem
    {
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<EnemyPath_Service> enemyPathService;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsWorldInject world;

        private readonly EcsFilterInject<
            Inc<IsTargetReached, Enemy, MovementToTarget, ObjectTransform, EnemyPath>,
            ExcludeNotAliveEnemies
        > entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var enemyEntity in entities.Value)
            {
                ref var enemy = ref entities.Pools.Inc2.Get(enemyEntity);
                ref var movement = ref entities.Pools.Inc3.Get(enemyEntity);
                ref var transform = ref entities.Pools.Inc4.Get(enemyEntity);
                ref var enemyPath = ref entities.Pools.Inc5.Get(enemyEntity);

                if (!levelMap.Value.HasCell(movement.target)) continue;

                // ref var currentCell = ref levelMap.Value.GetCell(movement.target);
                ref var currentCell = ref levelMap.Value.GetCell(transform.position);

                if (currentCell.isKernel)
                {
                    // Debug.Log($"Enemy reach kernel! [{enemyEntity}]");
                    events.Value.Entity.Add<Event_Enemy_ReachKernel>(enemyEntity, world.Value);
                    continue;
                }

                var path = enemyPathService.Value.GetPath(enemyEntity);
                
                var currentCoords = HexGridUtils.PositionToCell(transform.position.x, transform.position.y);

                var pathIndex = path.IndexOf(currentCoords);

                enemyPath.index = (ushort)pathIndex;

                /*Debug.Log($"currentCoords: {currentCoords}; pathItem: {path[enemyPath.index]}; nextPathItem: {path[enemyPath.index+1]}");

                if (enemyPath.index + 1 < path.Count && currentCoords != path[enemyPath.index + 1])
                {
                    Debug.Log("!!!AHTUNG!!!");
                    //currentCell = ref levelMap.Value.GetCell(path[enemyPath.index]);
                }
                else
                {
                    enemyPath.index++;
                }*/
                
                // if (currentCoords == path[enemyPath.index])
                // {
                    // enemyPath.index++;
                // }

                // Debug.Log("path index = " + enemyPath.index);
                if (enemyPath.index >= path.Count)
                {
                    // Debug.Log($"Enemy finished his path! [{enemyEntity}]");
                    continue;
                }

                // var stepCurrent = path[enemyPath.index];
                var stepNext = enemyPath.index + 1 < path.Count ? path[enemyPath.index + 1] : Int2.Zero;
                var stepNextNext = enemyPath.index + 2 < path.Count ? path[enemyPath.index + 2] : Int2.Zero;

                var nextCell = stepNext.IsZero || !levelMap.Value.HasCell(stepNext)
                    ? default
                    : levelMap.Value.GetCell(stepNext);
                var nextNextCell = stepNext.IsZero || !levelMap.Value.HasCell(stepNextNext)
                    ? default
                    : levelMap.Value.GetCell(stepNextNext);

                if (!nextCell.IsEmpty)
                {
                    var nextCellCoords = nextCell.coords;

                    var rotation = EnemyUtils.LookToNextCell(ref currentCell, ref nextCell);

                    if (!FloatUtils.IsEquals(transform.rotation.z, rotation.z) ||
                        !FloatUtils.IsEquals(transform.rotation.w, rotation.w))
                    {
                        var angularSpeed = EnemyUtils.GetAngularSpeed(ref enemy);

                        if (angularSpeed < Constants.Enemy.SmoothRotationThreshold)
                        {
                            ref var smoothRotation = ref common.Value.GetSmoothRotation(enemyEntity);
                            smoothRotation.from = transform.rotation;
                            smoothRotation.to = rotation;
                            smoothRotation.angularSpeed = angularSpeed;
                            // smoothRotation.targetBody = enemyMb.body;
                        }
                        else
                        {
                            transform.SetRotation(rotation);
                        }
                    }

                    // movement.from = movement.target;
                    movement.from = transform.position;
                    movement.target = EnemyUtils.CalcPosition(ref nextCellCoords, rotation, enemy.offset);
                    if (!nextNextCell.IsEmpty)
                    {
                        var rotationNextNext = EnemyUtils.LookToNextCell(ref nextCell, ref nextNextCell);
                        movement.nextTarget =
                            EnemyUtils.CalcPosition(ref nextNextCell.coords, rotationNextNext, enemy.offset);
                    }
                    else
                    {
                        movement.nextTarget = Vector2.zero;
                    }

                    // movement.SetSpeed(movement.speed); //, transform.rotation);
                    // Debug.Log("speedV = " + movement.speedV);

                    var fromToTargetV = (movement.target - movement.from);
                    movement.fromToTargetDistanse = fromToTargetV.magnitude;
                    enemy.distanceFromSpawn += movement.fromToTargetDistanse;

                    var normX = fromToTargetV.x / movement.fromToTargetDistanse;
                    var normY = fromToTargetV.y / movement.fromToTargetDistanse;
                    
                    movement.SetSpeed(movement.speed, normX * movement.speed, normY * movement.speed);
                    // movement.speedV.x = norm.x * movement.speed;
                    // movement.speedV.y = norm.y * movement.speed;
                }
            }
        }
    }
}