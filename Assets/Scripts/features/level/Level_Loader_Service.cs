using System;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.destroy;
using td.features.eventBus;
using td.features.level.cells;
using td.features.level.data;
using td.features.shard.shardCollection;
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
        [DI] private Level_Aspect aspect;
        [DI] private LevelMap levelMap;
        [DI] private State state;
        [DI] private Destroy_Service destroyService;
        [DI] private EventBus events;
        
        private GameObject levelGameObject;
        
        
        public bool HasLevel()
        {
            var check1 = Resources.Load<TextAsset>($"Levels/{state.GetLevelNumber()}") != null;
            var check2 = Resources.Load<GameObject>($"Levels/{state.GetLevelNumber()}") != null;

            return check1 && check2;
        }

        public void LoadLevel()
        {
            var ln = state.GetLevelNumber();
            state.Clear();
            state.SetLevelNumber(ln);
            
            ClearLastLevelData();
            
            LoadLevelConfig();
            LoadLevelPrefab();

            InitAllCells();
            levelMap.BuildMap();

            FixSpawnPoints();

#if UNITY_EDITOR
            // Debug.Log("FINAL MAP");
            levelMap.DebugLogHexMap();
#endif
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
            if (levelMap.LevelConfig == null) return;

            levelMap.Clear();

            // очищаем все события и команды во внешке
            // var outerEntities = Array.Empty<int>();
            // outerWorld.Value.GetAllEntities(ref outerEntities);
            // foreach (var outerEntity in outerEntities)
            // {
            //     outerWorld.Value.DelEntity(outerEntity);
            // }

            // удаляем все ентити которые живут только на уровне
            foreach (var entity in aspect.itOnlyOnLevel)
            {
                destroyService.SafeRemove(aspect.World().PackEntityWithWorld(entity));
            }
        }

        private void LoadLevelConfig()
        {
            var levelConfig = ResourcesUtils.LoadJson<LevelConfig>($"Levels/{state.GetLevelNumber()}");
            levelConfig.levelNumber = state.GetLevelNumber();
            levelMap.LevelConfig = levelConfig;
            
            state.SetMaxLives(Math.Max(1, levelConfig.startedLives));
            state.SetLives(Math.Max(1, state.GetMaxLives()));
            state.SetLevelNumber(levelConfig.levelNumber);
            state.SetEnergy(Math.Max(0, levelConfig.startedEnergy));
            state.SetNextWaveCountdown(0);
            state.SetActiveSpawnCount(0);
            state.SetWaveNumber(0);
            state.SetWaveCount(levelConfig.waves.Length);
            state.Ex<ShardCollection_StateExtension>().SetMaxItems(Math.Max((byte)4, levelConfig.maxShards));
        }

        private void LoadLevelPrefab()
        {
            if (levelGameObject != null)
            {
                Object.DestroyImmediate(levelGameObject);
            }

            levelGameObject = Object.Instantiate(
                Resources.Load<GameObject>($"Levels/{state.GetLevelNumber()}")
            );
        }

        private void InitAllCells()
        {
            foreach (var cell in Object.FindObjectsOfType<CellEditorMB>())
            {
                levelMap.PreAddCell(Cell.FromCellEditor(cell));
            }
        }
    }
}