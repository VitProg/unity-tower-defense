using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.attributes;
using td.components.behaviors;
using td.components.links;
using td.services;
using td.states;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Target = td.components.attributes.Target;

namespace td.features.enemies
{
    public class SpawnEnemyExecutor : IEcsRunSystem
    {
        [EcsInject] private LevelMap levelMap;
        [EcsInject] private LevelState levelState;
        [EcsShared] private SharedData shared;
        
        private readonly EcsFilterInject<Inc<SpawnEnemyOuterCommand>> eventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var outerWorld = systems.GetWorld(Constants.Worlds.Outer);

            foreach (var eventEntity in eventEntities.Value)
            {
                ref var spawnCommand = ref eventEntities.Pools.Inc1.Get(eventEntity);
                var enemyConfig = shared.GetEnemyConfig(spawnCommand.enemyName);

                var spawn = levelMap.Spawns[spawnCommand.spawner];
                var spawnCell = levelMap.GetCell(spawn.Coordinates);
                var nextCell = levelMap.GetCell(spawnCell.NextCellCoordinates);

                var containerForEnemies = GameObject.FindGameObjectWithTag(Constants.Tags.EnemiesContainer);
                
                var position = GridUtils.GetVector(spawn.Coordinates) + spawnCommand.offset;
                var rotation = EnemyUtils.LookToNextCell(spawnCell.Coordinates, nextCell.Coordinates);

                var enemyGameObject = Object.Instantiate(
                    enemyConfig.prefab,
                    position,
                    enemyConfig.prefab.transform.rotation,
                    containerForEnemies.transform
                );
                var enemyEntity = world.ConvertToEntity(enemyGameObject);
                
                enemyGameObject.transform.localScale = new Vector2(spawnCommand.scale, spawnCommand.scale);

                world.GetComponent<MoveToTarget>(enemyEntity).speed = spawnCommand.speed;
                world.GetComponent<MovableOffset>(enemyEntity).offset = spawnCommand.offset;
                world.AddComponent(enemyEntity, EnemyState.CreateFromSpawnCommand(spawnCommand));
                world.GetComponent<GameObjectLink>(enemyEntity).gameObject.transform.rotation = rotation;
                
                world.AddComponent( enemyEntity, new Target()
                {
                    target = EnemyUtils.TargetPosition(
                        nextCell.Coordinates,
                        rotation,
                        spawnCommand.offset
                    ),
                    gap = Constants.DefaultGap,
                });

                levelState.EnemiesCount++;

                outerWorld.DelEntity(eventEntity);
            }
        }
    }
}