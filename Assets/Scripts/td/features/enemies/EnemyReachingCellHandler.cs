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
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<ReachingTargetEvent>> entities = Constants.Ecs.EventsWorldName;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var eventsWorld = systems.GetWorld(Constants.Ecs.EventsWorldName);

            foreach (var eventEntity in entities.Value)
            {
                var packedEntity = entities.Pools.Inc1.Get(eventEntity).TargetEntity;

                if (
                    !packedEntity.Unpack(world, out var entity) ||
                    EntityUtils.HasComponent<IsEnemy>(systems, entity) == false
                )
                {
                    continue;
                }

                ref var target = ref EntityUtils.GetComponent<Target>(systems, entity);
                ref var movableOffset = ref EntityUtils.GetComponent<MovableOffset>(systems, entity);
                ref var gameObjectLink = ref EntityUtils.GetComponent<GameObjectLink>(systems, entity);

                var cell = levelData.Value.GetCell(target.target);
                var nextCell = levelData.Value.GetCell(cell.NextCellCoordinates);

                if (cell.isKernel)
                {
                    // send event
                    EntityUtils.AddComponent<IsEnemyDead>(systems, entity);
                    EcsEventUtils.Send(systems, new EnemyReachingKernelEvent()
                    {
                        EnemyEntity = world.PackEntity(entity)
                    });
                }
                else
                {
                    var rotation = EnemyUtils.LookToNextCell(cell.Coordinates, nextCell.Coordinates);

                    var transform = gameObjectLink.gameObject.transform;
                    
                    if (transform.rotation != rotation)
                    {
                        var angularSpeed = EnemyUtils.GetAngularSpeed(systems, entity);

                        if (angularSpeed < Constants.Enemy.SmoothRotationThreshold)
                        {
                            EntityUtils.AddComponent(systems, entity, new SmoothRotateCommand()
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

                eventsWorld.DelEntity(eventEntity);
            }
        }
    }
}