using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.attributes;
using td.components.behaviors;
using td.components.commands;
using td.components.links;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Target = td.components.attributes.Target;

namespace td.systems.waves
{
    public class SpawnEnemyExecutor : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsSharedInject<SharedData> shared = default;
        private readonly EcsFilterInject<Inc<SpawnEnemyCommand>> entities = Constants.Ecs.EventWorldName;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var eventsWorld = systems.GetWorld(Constants.Ecs.EventWorldName);

            foreach (var eventEntity in entities.Value)
            {
                ref var spawnConfig = ref entities.Pools.Inc1.Get(eventEntity);
                var enemyConfig = shared.Value.GetEnemyConfig(spawnConfig.enemyName);

                var spawn = levelData.Value.Spawns[spawnConfig.spawner];
                var spawnCell = levelData.Value.GetCell(spawn.Coordinates);
                var nextCell = levelData.Value.GetCell(spawnCell.NextCellCoordinates);

                var containerForEnemies = GameObject.FindGameObjectWithTag(Constants.Tags.EnemiesContainer);

                // var offset = new Vector2(
                    // (Random.Range(0, 100) - 50) / 100.0f,
                    // (Random.Range(0, 100) - 50) / 100.0f
                // );
                // var offset = spawnConfig.offset;

                var position = GridUtils.GetVector(spawn.Coordinates) + spawnConfig.offset;

                var enemyGameObject = Object.Instantiate(
                    enemyConfig.prefab,
                    position,
                    enemyConfig.prefab.transform.rotation,
                    containerForEnemies.transform
                );
                var entity = UniEcsUtils.Convert(enemyGameObject, world);

                // var scale = Random.Range(Constants.Enemy.MinSize, Constants.Enemy.MaxSize);
                enemyGameObject.transform.localScale = new Vector2(spawnConfig.scale, spawnConfig.scale);

                ref var moveToTargetPoint = ref EntityUtils.GetComponent<MoveToTarget>(systems, entity);
                moveToTargetPoint.speed = spawnConfig.speed;

                ref var movableOffset = ref EntityUtils.GetComponent<MovableOffset>(systems, entity);
                movableOffset.offset = spawnConfig.offset;

                EntityUtils.AddComponent(systems, entity, new Target()
                {
                    target = GridUtils.GetVector(nextCell.Coordinates),
                });

                EntityUtils.AddComponent<SpawnEnemyCommand>(systems, entity) = spawnConfig;

                //todo
                var toNextCellVector = GridUtils.GetVector(nextCell.Coordinates) -
                                       GridUtils.GetVector(spawnCell.Coordinates);
                toNextCellVector.Normalize();
                ref var gameObjectLink = ref EntityUtils.GetComponent<GameObjectLink>(systems, entity);
                gameObjectLink.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, toNextCellVector);

                eventsWorld.DelEntity(eventEntity);
            }
        }
    }
}