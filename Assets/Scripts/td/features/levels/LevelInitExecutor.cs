using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.levels
{
    // todo rewrite to load level service
    public class LevelInitExecutor: IEcsRunSystem
    {
        [EcsInject] private LevelMap levelMap;
        
        private readonly EcsFilterInject<Inc<LevelInitOuterCommand>> eventEntities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<Test>> test = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<LevelInitOC>> LevelInitOC = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                RunInternal(systems, eventEntity);
                
                systems.CleanupOuter(eventEntities);
                break;
            }
        }

        private void RunInternal(IEcsSystems systems, int entity)
        {
            // Debug.Log("LevelInitExecutor RUN...");

            // Tiles
            InitTiles();

            // Spawns
            InitSpawns();

            // Target
            InitKernels();

#if UNITY_EDITOR
            Debug.Log("FINAL MAP");
            levelMap.ShowMapInLog();
#endif

            systems.SendOuter<PathInitOuterCommand>();
            
            // Debug.Log("LevelInitExecutor FIN");
        }

        private void InitTiles()
        {
            var tiles = GameObject.FindGameObjectsWithTag(Constants.Tags.Tile);
            foreach (var tileGameObject in tiles)
            {
                var cell = new Cell
                {
                    Coordinates = GridUtils.GetGridCoordinate(tileGameObject.transform.position),
                    walkable = true,
                    space = false,
                    buildEntity = -1,
                    gameObject = tileGameObject,
                };

                levelMap.AddCell(cell);
            }
        }

        private void InitSpawns()
        {
            var spawns = GameObject.FindGameObjectsWithTag(Constants.Tags.Spawn);
            foreach (var spawnGameObject in spawns)
            {
                var spawn = new Spawn
                {
                    Coordinates = GridUtils.GetGridCoordinate(spawnGameObject.transform.position),
                };
                levelMap.AddSpawn(spawn);
            }
        }

        private void InitKernels()
        {
            var targets = GameObject.FindGameObjectsWithTag(Constants.Tags.Target);
            foreach (var targetGameObject in targets)
            {
                var kernel = new Kernel()
                {
                    Coordinates = GridUtils.GetGridCoordinate(targetGameObject.transform.position),
                };
                levelMap.AddKernel(kernel);
            }
        }
    }
}