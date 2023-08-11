using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.systems;
using td.features.level.cells;
using td.features.level.data;
using td.features.state;
using td.monoBehaviours;
using td.utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.level
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LevelLoader_Service
    {
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Common_Pools> commonPools;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsWorldInject world;
        
        private GameObject levelGameObject;
        
        
        public bool HasLevel()
        {
            var check1 = Resources.Load<TextAsset>($"Levels/{state.Value.LevelNumber}") != null;
            var check2 = Resources.Load<GameObject>($"Levels/{state.Value.LevelNumber}") != null;

            return check1 && check2;
        }

        public void LoadLevel()
        {
            try
            {
                var ln = state.Value.LevelNumber;
                state.Value.Clear();
                state.Value.LevelNumber = ln;
                
                ClearLastLevelData();
                
                LoadLevelConfig();
                LoadLevelPrefab();

                InitAllCells();
                levelMap.Value.BuildMap();

                FixSpawnPoints();

#if UNITY_EDITOR
                Debug.Log("FINAL MAP");
                levelMap.Value.DebugLogHexMap();
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
            
            // foreach (var spawn in levelMap.Value.Spawns)
            // {
            //     
            // }
        }


        private void ClearLastLevelData()
        {
            if (levelMap.Value.LevelConfig == null) return;

            levelMap.Value.Clear();

            // очищаем все события и команды во внешке
            // var outerEntities = Array.Empty<int>();
            // outerWorld.Value.GetAllEntities(ref outerEntities);
            // foreach (var outerEntity in outerEntities)
            // {
            //     outerWorld.Value.DelEntity(outerEntity);
            // }

            // удаляем все ентити которые живут только на уровне
            foreach (var entity in commonPools.Value.onlyOnLevelFilter.Value)
            {
                common.Value.Remove(entity);
            }
        }

        private void LoadLevelConfig()
        {
            var levelConfig = ResourcesUtils.LoadJson<LevelConfig>($"Levels/{state.Value.LevelNumber}");
            levelConfig.levelNumber = state.Value.LevelNumber;
            levelMap.Value.LevelConfig = levelConfig;
            
            state.Value.MaxLives = Math.Max(1, levelConfig.startedLives);
            state.Value.Lives = Math.Max(1, state.Value.MaxLives);
            state.Value.LevelNumber = levelConfig.levelNumber;
            state.Value.Energy = Math.Max(0, levelConfig.startedEnergy);
            state.Value.NextWaveCountdown = 0;
            state.Value.ActiveSpawnCount = 0;
            state.Value.WaveNumber = 0;
            state.Value.WaveCount = levelConfig.waves.Length;
            state.Value.ShardCollection.MaxItems = Math.Max((byte)4, levelConfig.maxShards);
        }

        private void LoadLevelPrefab()
        {
            if (levelGameObject != null)
            {
                Object.DestroyImmediate(levelGameObject);
            }

            levelGameObject = Object.Instantiate(
                Resources.Load<GameObject>($"Levels/{state.Value.LevelNumber}")
            );
        }

        private void InitAllCells()
        {
            foreach (var cell in Object.FindObjectsOfType<CellEditorMB>())
            {
                levelMap.Value.PreAddCell(Cell.FromCellEditor(cell));
            }
        }
    }
}