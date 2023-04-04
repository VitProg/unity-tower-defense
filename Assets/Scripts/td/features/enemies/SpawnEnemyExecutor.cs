using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.attributes;
using td.components.behaviors;
using td.components.links;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Target = td.components.attributes.Target;

namespace td.features.enemies
{
    public class SpawnEnemyExecutor : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsSharedInject<SharedData> shared = default;
        private readonly EcsFilterInject<Inc<SpawnEnemyCommand>> entities = Constants.Ecs.EventsWorldName;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var eventsWorld = systems.GetWorld(Constants.Ecs.EventsWorldName);

            foreach (var eventEntity in entities.Value)
            {
                ref var spawnConfig = ref entities.Pools.Inc1.Get(eventEntity);
                var enemyConfig = shared.Value.GetEnemyConfig(spawnConfig.enemyName);

                var spawn = levelData.Value.Spawns[spawnConfig.spawner];
                var spawnCell = levelData.Value.GetCell(spawn.Coordinates);
                var nextCell = levelData.Value.GetCell(spawnCell.NextCellCoordinates);

                var containerForEnemies = GameObject.FindGameObjectWithTag(Constants.Tags.EnemiesContainer);

                var position = GridUtils.GetVector(spawn.Coordinates) + spawnConfig.offset;

                var enemyGameObject = Object.Instantiate(
                    enemyConfig.prefab,
                    position,
                    enemyConfig.prefab.transform.rotation,
                    containerForEnemies.transform
                );
                var entity = UniEcsUtils.Convert(enemyGameObject, world);
                
                enemyGameObject.transform.localScale = new Vector2(spawnConfig.scale, spawnConfig.scale);

                ref var moveToTargetPoint = ref EntityUtils.GetComponent<MoveToTarget>(systems, entity);
                moveToTargetPoint.speed = spawnConfig.speed;

                ref var movableOffset = ref EntityUtils.GetComponent<MovableOffset>(systems, entity);
                movableOffset.offset = spawnConfig.offset;

                EntityUtils.AddComponent<SpawnEnemyCommand>(systems, entity) = spawnConfig;

                ref var gameObjectLink = ref EntityUtils.GetComponent<GameObjectLink>(systems, entity);
                
                var rotation = EnemyUtils.LookToNextCell(spawnCell.Coordinates, nextCell.Coordinates);

                gameObjectLink.gameObject.transform.rotation = rotation;
                
                EntityUtils.AddComponent(systems, entity, new Target()
                {
                    target = EnemyUtils.TargetPosition(
                        nextCell.Coordinates,
                        rotation,
                        movableOffset.offset
                    ),
                    gap = Constants.DefaultGap,
                });

                eventsWorld.DelEntity(eventEntity);
            }
        }
    }
}