using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.behaviors;
using td.components.refs;
using td.services;
using td.states;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies
{
    public class SpawnEnemyExecutor : IEcsRunSystem, IEcsInitSystem
    {
        [EcsInject] private LevelMap levelMap;
        [EcsInject] private LevelState levelState;
        [EcsShared] private SharedData shared;

        private readonly EcsFilterInject<Inc<SpawnEnemyOuterCommand>> eventEntities = Constants.Worlds.Outer;
        private GameObject containerForEnemies;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var outerWorld = systems.GetWorld(Constants.Worlds.Outer);

            foreach (var eventEntity in eventEntities.Value)
            {
                ref var spawnCommand = ref eventEntities.Pools.Inc1.Get(eventEntity);
                var enemyConfig = shared.GetEnemyConfig(spawnCommand.enemyName);
                
                if (enemyConfig == null) continue;

                var spawn = levelMap.GetSpawn(spawnCommand.spawner);
                
                if (
                    !levelMap.TryGetCell(spawn, out var spawnCell) ||
                    !levelMap.TryGetCell(spawnCell.GetRandomNextCoords(), out var nextCell)
                ) continue;

                var rotation = EnemyUtils.LookToNextCell(spawnCell, nextCell);
                var position = EnemyUtils.Position(spawnCell.Coords, rotation, spawnCommand.offset);

                var enemyGameObject = Object.Instantiate(
                    enemyConfig.Value.prefab,
                    position,
                    enemyConfig.Value.prefab.transform.rotation,
                    containerForEnemies.transform
                );
                var enemyEntity = world.ConvertToEntity(enemyGameObject);

                enemyGameObject.transform.rotation = rotation;

                ref var go = ref world.GetComponent<Ref<GameObject>>(enemyEntity);
                go.reference = enemyGameObject;

                world.GetComponent<Enemy>(enemyEntity).Setup(
                    position,
                    rotation,
                    spawnCommand
                );

                world.AddComponent(enemyEntity, new LinearMovementToTarget
                {
                    from = position,
                    target = EnemyUtils.Position(
                        nextCell.Coords,
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

        public void Init(IEcsSystems systems)
        {
            containerForEnemies = GameObject.FindGameObjectWithTag(Constants.Tags.EnemiesContainer);
        }
    }
}