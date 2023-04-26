using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using td.common.level;
using td.components.flags;
using td.components.refs;
using td.features.towers;
using td.monoBehaviours;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.services
{
    public class LevelLoader
    {
        [Inject] private LevelMap levelMap;
        [Inject] private LevelState levelState;
        // [Inject] private EntityConverters converters;

        private GameObject levelGameObject;
        
        
        public bool HasLevel()
        {
            var check1 = Resources.Load<TextAsset>($"Levels/{levelState.LevelNumber}") != null;
            var check2 = Resources.Load<GameObject>($"Levels/{levelState.LevelNumber}") != null;

            return check1 && check2;
        }

        public void LoadLevel(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            // ToDo: levelMap.clear();
            try
            {
                levelState.ClearForNewLevel();

                ClearLastLevelData(systems);

                LoadLevelConfig();
                LoadLevelPrefab();

                InitAllCells();
                levelMap.BuildMap();

                FixSpawnPoints();

#if UNITY_EDITOR
                Debug.Log("FINAL MAP");
                levelMap.DebugLogHexMap();
#endif
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void FixSpawnPoints()
        {
            // levelMap.LevelConfig?.waves[0].spawns[0].spawner
            
            foreach (var spawn in levelMap.Spawns)
            {
                
            }
        }


        private void ClearLastLevelData(IEcsSystems systems)
        {
            if (levelMap.LevelConfig == null) return;

            levelMap.Clear();

            var world = systems.GetWorld();
            var outerWorld = systems.GetWorld(Constants.Worlds.Outer);

            // очищаем все события и команды во внешке
            var outerEntities = Array.Empty<int>();
            outerWorld.GetAllEntities(ref outerEntities);
            foreach (var outerEntity in outerEntities)
            {
                world.DelEntity(outerEntity);
                ;
            }

            // удаляем все ентити которые живут только на уровне
            var entities = world.Filter<OnlyOnLevel>().End();
            foreach (var entity in entities)
            {
                // также удаляем привязанный GameObject
                if (world.HasComponent<Ref<GameObject>>(entity))
                {
                    var refGameObject = world.GetComponent<Ref<GameObject>>(entity);
                    Object.Destroy(refGameObject.reference);
                }

                world.DelEntity(entity);
            }
        }

        private void LoadLevelConfig()
        {
            var levelConfig = ResourcesUtils.LoadJson<LevelConfig>($"Levels/{levelState.LevelNumber}");
            levelConfig.levelNumber = levelState.LevelNumber;
            levelMap.LevelConfig = levelConfig;
        }

        private void LoadLevelPrefab()
        {
            if (levelGameObject != null)
            {
                Object.DestroyImmediate(levelGameObject);
            }

            levelGameObject = Object.Instantiate(
                Resources.Load<GameObject>($"Levels/{levelState.LevelNumber}")
            );
        }

        private void InitAllCells()
        {
            foreach (var cell in Object.FindObjectsOfType<Cell>())
            {
                levelMap.PreAddCell(cell);
            }
        }


        // public void InitBuildings(EcsWorld world)
        // {
        //     var towerPool = world.GetPool<Tower>();
        //     var goPool = world.GetPool<Ref<GameObject>>();
        //
        //     foreach (var towerTransform in Object.FindObjectsOfType<TowerProvider>())
        //     {
        //         // var entity = world.ConvertToEntity(toerTransform.gameObject);
        //         var entity = world.NewEntity();
        //         if (converters.Convert<Tower>(towerTransform.gameObject, entity))
        //         {
        //             var tower = towerPool.Get(entity);
        //             var towerGameObject = goPool.Get(entity);
        //
        //             var cellCoordinates = HexGridUtils.PositionToCell(towerGameObject.reference.transform.position);
        //
        //             var cell = levelMap.GetCell(cellCoordinates, CellTypes.CanBuild);
        //
        //             if (cell != null)
        //             {
        //                 // ToDo
        //                 cell.Buildings[0] = world.PackEntity(entity);
        //             }
        //
        //             if (tower.radiusGameObject == null)
        //             {
        //                 var radiusTransform = towerGameObject.reference.transform.Find("radius");
        //                 if (radiusTransform != null)
        //                 {
        //                     tower.radiusGameObject = radiusTransform.gameObject;
        //                     tower.radiusGameObject.SetActive(false);
        //                 }
        //             }
        //         }
        //     }
        // }
    }
}