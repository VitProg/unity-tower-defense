using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using td.common.level;
using td.components.flags;
using td.components.refs;
using td.features.state;
using td.features.towers;
using td.monoBehaviours;
using td.systems.commands;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.services
{
    public class LevelLoader
    {
        [Inject] private LevelMap levelMap;
        [Inject] private State state;
        // [Inject] private EntityConverters converters;

        private GameObject levelGameObject;
        
        
        public bool HasLevel()
        {
            var check1 = Resources.Load<TextAsset>($"Levels/{state.LevelNumber}") != null;
            var check2 = Resources.Load<GameObject>($"Levels/{state.LevelNumber}") != null;

            return check1 && check2;
        }

        public void LoadLevel(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            // ToDo: levelMap.clear();
            try
            {
                var ln = state.LevelNumber;
                state.SuspendEvents();
                state.Clear();
                state.LevelNumber = ln;
                state.EnemiesCount = 0;
                state.WaveCount = 0;
                state.WaveNumber = 0;
                state.IsBuildingProcess = false;
                state.NextWaveCountdown = 0;
                state.ResumeEvents();

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
                    var go = world.GetComponent<Ref<GameObject>>(entity).reference;
                    RemoveGameObjectExecutor.Remove(go, entity);
                    // Object.Destroy(refGameObject.reference);
                }

                world.DelEntity(entity);
            }
        }

        private void LoadLevelConfig()
        {
            var levelConfig = ResourcesUtils.LoadJson<LevelConfig>($"Levels/{state.LevelNumber}");
            levelConfig.levelNumber = state.LevelNumber;
            levelMap.LevelConfig = levelConfig;
        }

        private void LoadLevelPrefab()
        {
            if (levelGameObject != null)
            {
                Object.DestroyImmediate(levelGameObject);
            }

            levelGameObject = Object.Instantiate(
                Resources.Load<GameObject>($"Levels/{state.LevelNumber}")
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
        //                 cell.L3_Buildings[0] = world.PackEntity(entity);
        //             }
        //
        //             if (tower.radiusGameObject == null)
        //             {
        //                 var radiusTransform = towerGameObject.reference.transform.Find("outerRadius");
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