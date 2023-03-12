using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.components.attributes;
using td.components.commands;
using td.components.events;
using td.components.flags;
using td.components.links;
using td.services;
using td.systems.commands;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.events
{
    public class EnemyReachingCellHandler : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<ReachingTargetEvent>> entities = Constants.Ecs.EventWorldName;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var eventsWorld = systems.GetWorld(Constants.Ecs.EventWorldName);

            foreach (var eventEntity in entities.Value)
            {
                var packedEntity = entities.Pools.Inc1.Get(eventEntity).TargetEntity;

                if (
                    !packedEntity.Unpack(world, out var entity) ||
                    EntityUtils.HasComponent<IsEnemy>(systems, entity) == false
                )
                {
                    eventsWorld.DelEntity(eventEntity);
                    continue;
                }

                ref var target = ref EntityUtils.GetComponent<Target>(systems, entity);
                ref var movableOffset = ref EntityUtils.GetComponent<MovableOffset>(systems, entity);
                ref var transformLink = ref EntityUtils.GetComponent<TransformLink>(systems, entity);

                var cell = levelData.Value.GetCell(target.target);
                var nextCell = levelData.Value.GetCell(cell.NextCellCoordinates);


                if (!cell.isTarget)
                {
                    var toNextCellVector = GridUtils.GetVector(nextCell.Coordinates) - GridUtils.GetVector(cell.Coordinates);
                    toNextCellVector.Normalize();
                    var rotation = Quaternion.LookRotation(Vector3.forward, toNextCellVector);

                    if (transformLink.transform.rotation != rotation)
                    {
                        var angularSpeed = 5f;
                        if (EntityUtils.HasComponent<SpawnEnemyCommand>(systems, entity))
                        {
                            ref var spawConfig = ref EntityUtils.GetComponent<SpawnEnemyCommand>(systems, entity);
                            angularSpeed = spawConfig.angularSpeed > 0.01f 
                                ? spawConfig.angularSpeed
                                : 5f;
                        }

                        if (angularSpeed < 100f)
                        {
                            EntityUtils.AddComponent(systems, entity, new SmoothRotateCommand()
                            {
                                From = transformLink.transform.rotation,
                                To = rotation,
                                AngularSpeed = angularSpeed
                            });
                        }
                        else
                        {
                            transformLink.transform.rotation = rotation;
                        }
                    }

                    var newTarget = GridUtils.GetVector(nextCell.Coordinates) +
                                    (Vector2)(rotation * movableOffset.offset);
                    target.target = newTarget;
                }

                if (cell.isTarget) // todo - rename it to "kernel" or "core" or "home"...
                {
                    // TODO
                    EntityUtils.AddComponent<RemoveGameObjectCommand>(systems, entity);
                }

                eventsWorld.DelEntity(eventEntity);
            }
        }
    }
}