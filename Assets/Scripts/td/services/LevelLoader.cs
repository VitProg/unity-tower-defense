using System;
using Leopotam.EcsLite;
using td.common;
using td.common.level;
using td.features.towers;
using td.utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.services
{
    public class LevelLoader
    {
        private readonly LevelMap levelMap;
        
        public LevelLoader(LevelMap levelMap)
        {
            this.levelMap = levelMap;
        }
        
        public void LoadLevel(int levelNumber = 1)
        {
            // ToDo: levelMap.clear();
            try
            {
                LoadLevelConfig(levelNumber);
                LoadLevelPrefab(levelNumber);
                
                InitTiles();
                InitSpawns();
                InitKernels();
                
#if UNITY_EDITOR
                Debug.Log("FINAL MAP");
                levelMap.ShowMapInLog();
#endif
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void LoadLevelConfig(int levelNumber)
        {
            levelMap.LevelConfig = ResourcesUtils.LoadJson<LevelConfig>($"Levels/{levelNumber}");
        }

        private void LoadLevelPrefab(int levelNumber)
        {
            Object.Instantiate(
                Resources.Load<GameObject>($"Levels/{levelNumber}")
            );
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
                var spawn = new Spawn()
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

        private void InitBuildings(EcsWorld world)
        {
            var towerPool = world.GetPool<Tower>();
            var goPool = world.GetPool<Ref<GameObject>>();
            var entities = world.Filter<Tower>().Inc<Ref<GameObject>>().End();
            
            foreach (var entity in entities)
            {
                var tower = towerPool.Get(entity);
                var towerGameObject = goPool.Get(entity);
                
                // var radius = towerGameObject.reference.transform.Find("radius");
                // radius.gameObject.hideFlags = HideFlags.None;
                // radius.localScale = new Vector3(tower.radius, tower.radius, tower.radius) * 1.3f;
            }
        }
    }
}