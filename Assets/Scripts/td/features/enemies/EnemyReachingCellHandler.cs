using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.attributes;
using td.components.commands;
using td.components.events;
using td.components.flags;
using td.components.links;
using td.services;
using td.utils;
using td.utils.ecs;

namespace td.features.enemies
{
    public class EnemyReachingCellHandler : IEcsRunSystem
    {
        [EcsInject] private LevelMap levelMap;
        [EcsWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<ReachingTargetEvent, IsEnemy>> entities;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var target = ref world.GetComponent<Target>(entity);
                ref var movableOffset = ref world.GetComponent<MovableOffset>(entity);
                ref var gameObjectLink = ref world.GetComponent<GameObjectLink>(entity);

                var cell = levelMap.GetCell(target.target);
                var nextCell = levelMap.GetCell(cell.NextCellCoordinates);

                if (cell.IsKernel)
                {
                    // send event
                    world.AddComponent<IsEnemyDead>(entity);
                    world.AddComponent<EnemyReachingKernelEvent>(entity);
                }
                else
                {
                    var rotation = EnemyUtils.LookToNextCell(cell.Coordinates, nextCell.Coordinates);

                    var transform = gameObjectLink.gameObject.transform;
                    
                    if (transform.rotation != rotation)
                    {
                        var angularSpeed = EnemyUtils.GetAngularSpeed(world, entity);

                        if (angularSpeed < Constants.Enemy.SmoothRotationThreshold)
                        {
                            world.AddComponent(entity, new SmoothRotateCommand()
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

                    target.target = EnemyUtils.TargetPosition(
                        nextCell.Coordinates,
                        rotation,
                        movableOffset.offset
                    );;
                }
            }
        }
    }
}