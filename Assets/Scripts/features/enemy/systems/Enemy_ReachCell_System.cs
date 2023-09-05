using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy.bus;
using td.features.enemy.enemyPath;
using td.features.eventBus;
using td.features.level;
using td.features.movement;
using td.utils;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.enemy.systems {
    public class Enemy_ReachCell_System : IProtoRunSystem {
        [DI] private Enemy_Aspect aspect;
        [DI] private Level_State levelState;
        [DI] private Enemy_Service enemyService;
        [DI] private EnemyPath_Service enemyPathService;
        [DI] private EnemyPath_State enemyPathState;
        [DI] private Movement_Service movementService;
        [DI] private EventBus events;

        public void Run() {
            foreach (var enemyEntity in aspect.itEnemiesReachingCell) {
                ref var movement = ref movementService.GetMovement(enemyEntity);
                
                if (!levelState.HasCell(movement.target)) continue;
                
                ref var transform = ref movementService.GetTransform(enemyEntity);
                ref var currentCell = ref levelState.GetCell(transform.position);

                if (currentCell.isKernel) {
                    events.global.Add<Event_Enemy_ReachKernel>().Entity = aspect.World().PackEntityWithWorld(enemyEntity);
                    continue;
                }
                
                ref var route = ref enemyPathService.GetRoute(enemyEntity);
                var routeLength = enemyPathState.GetRouteLength(route.routeIdx);
                
                var currentCellCoord = HexGridUtils.PositionToCell(transform.position.x, transform.position.y);
                if (!enemyPathState.GetRouteItem(route.routeIdx, route.step).Equals(currentCellCoord)) {
                    var step = enemyPathState.GetRouteItemIndexByCoord(route.routeIdx, currentCellCoord.x, currentCellCoord.y);
                    route.step = step;
                }

                if (route.step >= routeLength) {
                    continue;
                }

                var hasStepNext = enemyPathState.HasRouteItem(route.routeIdx, route.step + 1);
                var hasStepNextNext = enemyPathState.HasRouteItem(route.routeIdx, route.step + 2);
                var stepNext = hasStepNext ? enemyPathState.GetRouteItem(route.routeIdx, route.step + 1) : int2.zero;
                var stepNextNext = hasStepNextNext ? enemyPathState.GetRouteItem(route.routeIdx, route.step + 2) : int2.zero;

                // todo need ref
                var nextCell = hasStepNext || !levelState.HasCell(stepNext)
                    ? default
                    : levelState.GetCell(stepNext);
                var nextNextCell =hasStepNextNext || !levelState.HasCell(stepNextNext)
                    ? default
                    : levelState.GetCell(stepNextNext);

                if (!nextCell.IsEmpty) {
                    var nextCellCoords = nextCell.coords;

                    var rotation = Enemy_Utils.LookToNextCell(ref currentCell, ref nextCell);

                    ref var enemy = ref aspect.enemyPool.Get(enemyEntity);
                    
                    if (!FloatUtils.IsEquals(transform.rotation.value.z, rotation.z) ||
                        !FloatUtils.IsEquals(transform.rotation.value.w, rotation.w)) {
                        var angularSpeed = Enemy_Utils.GetAngularSpeed(ref enemy);

                        if (angularSpeed < Constants.Enemy.SmoothRotationThreshold) {
                            ref var smoothRotation = ref movementService.GetSmoothRotation(enemyEntity);
                            smoothRotation.from = transform.rotation;
                            smoothRotation.to = rotation;
                            smoothRotation.angularSpeed = angularSpeed;
                            // smoothRotation.targetBody = enemyMb.body;
                        } else {
                            transform.SetRotation(rotation);
                        }
                    }

                    // movement.from = movement.target;
                    movement.from = transform.position;
                    movement.target = Enemy_Utils.CalcPosition(ref nextCellCoords, rotation, enemy.offset);
                    if (!nextNextCell.IsEmpty) {
                        var rotationNextNext = Enemy_Utils.LookToNextCell(ref nextCell, ref nextNextCell);
                        movement.nextTarget =
                            Enemy_Utils.CalcPosition(ref nextNextCell.coords, rotationNextNext, enemy.offset);
                    } else {
                        movement.nextTarget = Vector2.zero;
                    }


                    var fromToTargetV = (movement.target - movement.from);
                    movement.fromToTargetDistanse = fromToTargetV.Magnitude();
                    enemy.distanceFromSpawn += movement.fromToTargetDistanse;

                    var normX = fromToTargetV.x / movement.fromToTargetDistanse;
                    var normY = fromToTargetV.y / movement.fromToTargetDistanse;

                    movement.SetSpeed(movement.speed, normX * movement.speed, normY * movement.speed);
                }
            }
        }
    }
}
