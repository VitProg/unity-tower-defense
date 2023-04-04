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
    public class LevelInitExecutor: IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<LevelInitCommand>> entities = Constants.Ecs.EventsWorldName;

        public void Run(IEcsSystems systems)
        {
            if(EcsEventUtils.FirstEntity(entities) == null) return;
            
            Debug.Log("LevelInitExecutor RUN...");

            // Tiles
            InitTiles();

            // Spawns
            InitSpawns();

            // Target
            InitTargets();

            EcsEventUtils.CleanupEvent(systems, entities);
            
            EcsEventUtils.Send<PathInitCommand>(systems);
            
            Debug.Log("LevelInitExecutor FIN");
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

                levelData.Value.AddCell(cell);
            }

            var line = "";
            for (var y = 0; y < levelData.Value.Height; y++)
            {
                for (var x = 0; x < levelData.Value.Width; x++)
                {
                    line += levelData.Value.GetCell(x, y) != null ? '#' : '0';
                }
                line += '\n';
            }
            Debug.Log(line);
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
                levelData.Value.AddSpawn(spawn);
            }
        }

        private void InitTargets()
        {
            var targets = GameObject.FindGameObjectsWithTag(Constants.Tags.Target);
            foreach (var targetGameObject in targets)
            {
                var kernel = new Kernel()
                {
                    Coordinates = GridUtils.GetGridCoordinate(targetGameObject.transform.position),
                };
                levelData.Value.AddKernel(kernel);
            }
        }
    }
}