using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.common.cells;
using td.common.cells.interfaces;
using td.common.level;
using td.components;
using td.components.behaviors;
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

                var spawn = levelMap.Spawns[spawnCommand.spawner];
                
                if (
                    !levelMap.TryGetCell<ICellCanWalk>(spawn.Coordinates, out var spawnCell) ||
                    !levelMap.TryGetCell<ICellCanWalk>(spawnCell.NextCellCoordinates, out var nextCell)
                ) continue;

                var position = GridUtils.CellToCoords(spawn.Coordinates, levelMap.CellType, levelMap.CellSize) + spawnCommand.offset;
                var rotation = EnemyUtils.LookToNextCell(spawnCell.Coordinates, nextCell.Coordinates, levelMap.CellType, levelMap.CellSize);

                var enemyGameObject = Object.Instantiate(
                    enemyConfig.prefab,
                    position,
                    enemyConfig.prefab.transform.rotation,
                    containerForEnemies.transform
                );
                var enemyEntity = world.ConvertToEntity(enemyGameObject);

                var etalonScale = levelMap.CellType == LevelCellType.Hex ? levelMap.CellSize / 1.1f : levelMap.CellSize;
                enemyGameObject.transform.localScale = Vector2.one * (etalonScale * spawnCommand.scale);

                enemyGameObject.transform.rotation = rotation;

                ref var go = ref world.GetComponent<Ref<GameObject>>(enemyEntity);
                go.reference = enemyGameObject;

                world.GetComponent<Enemy>(enemyEntity).Setup(
                    position,
                    rotation,
                    spawnCommand
                );

                world.AddComponent(enemyEntity, new LinearMovementToTarget()
                {
                    target = EnemyUtils.TargetPosition(
                        nextCell.Coordinates,
                        rotation,
                        spawnCommand.offset,
                        levelMap.CellType,
                        levelMap.CellSize
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