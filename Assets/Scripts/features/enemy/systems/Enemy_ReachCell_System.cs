using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.common;
using td.features.enemy.bus;
using td.features.eventBus;
using td.features.level;
using td.features.movement;
using td.utils;
using UnityEngine;

namespace td.features.enemy.systems
{
    public class Enemy_ReachCell_System : IProtoRunSystem
    {
        [DI] private Enemy_Aspect aspect;
        [DI] private LevelMap levelMap;
        [DI] private Enemy_Service enemyService;
        [DI] private Enemy_Path_Service enemyPathService;
        [DI] private Movement_Service movementService;
        [DI] private EventBus events;

        public void Run()
        {
            foreach (var enemyEntity in aspect.itEnemiesReachingCell)
            {
                ref var enemy = ref aspect.enemyPool.Get(enemyEntity);
                ref var enemyPath = ref aspect.enemyPathPool.Get(enemyEntity);
                ref var movement = ref movementService.GetMovement(enemyEntity);
                ref var transform = ref movementService.GetTransform(enemyEntity);

                if (!levelMap.HasCell(movement.target)) continue;

                // ref var currentCell = ref levelMap.Value.GetCell(movement.target);
                ref var currentCell = ref levelMap.GetCell(transform.position);

                if (currentCell.isKernel)
                {
                    // Debug.Log($"Enemy reach kernel! [{enemyEntity}]");
                    events.global.Add<Event_Enemy_ReachKernel>().Entity =
                        aspect.World().PackEntityWithWorld(enemyEntity);
                    continue;
                }

                var path = enemyPathService.GetPath(enemyEntity);

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

                var nextCell = stepNext.IsZero || !levelMap.HasCell(stepNext)
                    ? default
                    : levelMap.GetCell(stepNext);
                var nextNextCell = stepNext.IsZero || !levelMap.HasCell(stepNextNext)
                    ? default
                    : levelMap.GetCell(stepNextNext);

                if (!nextCell.IsEmpty)
                {
                    var nextCellCoords = nextCell.coords;

                    var rotation = Enemy_Utils.LookToNextCell(ref currentCell, ref nextCell);

                    if (!FloatUtils.IsEquals(transform.rotation.z, rotation.z) ||
                        !FloatUtils.IsEquals(transform.rotation.w, rotation.w))
                    {
                        var angularSpeed = Enemy_Utils.GetAngularSpeed(ref enemy);

                        if (angularSpeed < Constants.Enemy.SmoothRotationThreshold)
                        {
                            ref var smoothRotation = ref movementService.GetSmoothRotation(enemyEntity);
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
                    movement.target = Enemy_Utils.CalcPosition(ref nextCellCoords, rotation, enemy.offset);
                    if (!nextNextCell.IsEmpty)
                    {
                        var rotationNextNext = Enemy_Utils.LookToNextCell(ref nextCell, ref nextNextCell);
                        movement.nextTarget =
                            Enemy_Utils.CalcPosition(ref nextNextCell.coords, rotationNextNext, enemy.offset);
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