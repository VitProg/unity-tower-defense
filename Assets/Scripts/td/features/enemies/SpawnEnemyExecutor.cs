using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.behaviors;
using td.services;
using td.states;
using td.utils;
using td.utils.ecs;
using UnityEngine;

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

                enemyGameObject.transform.rotation = rotation;
                
                ref var go = ref world.GetComponent<Ref<GameObject>>(enemyEntity);
                go.reference = enemyGameObject;

                ref var enemy = ref world.GetComponent<Enemy>(enemyEntity);
                enemy.position = position;
                enemy.rotation = rotation;
                enemy.enemyName = spawnCommand.enemyName;
                enemy.spawner = spawnCommand.spawner;
                enemy.speed = spawnCommand.speed;
                enemy.angularSpeed = spawnCommand.angularSpeed;
                enemy.health = spawnCommand.health;
                enemy.damage = spawnCommand.damage;
                enemy.scale = spawnCommand.scale;
                enemy.offset = spawnCommand.offset;
                enemy.money = spawnCommand.money;
                
                world.AddComponent( enemyEntity, new LinearMovementToTarget()
                {
                    target = EnemyUtils.TargetPosition(
                        nextCell.Coordinates,
                        rotation,
                        spawnCommand.offset
                    ),
                    speed = spawnCommand.speed,
                    gap = Constants.DefaultGap,
                });

                levelState.EnemiesCount++;

                outerWorld.DelEntity(eventEntity);
            }
        }
    }
}