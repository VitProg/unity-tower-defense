using Leopotam.EcsProto.QoL;
using td.features._common.data;
using td.features.camera;
using td.features.destroy;
using td.features.eventBus;
using td.features.level.cells;
using td.features.state;
using td.utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.level
{
    public class LevelLoader_Service
    {
        [DI] private State state;
        [DI] private Destroy_Service destroyService;
        [DI] private EventBus events;
        [DI] private Camera_Service cameraService;
        [DI] private Level_State levelState;
        
        private GameObject levelGameObject;
        
        
        public bool HasLevel()
        {
            var check1 = Resources.Load<TextAsset>($"Levels/{levelState.GetLevelNumber()}") != null;
            var check2 = Resources.Load<GameObject>($"Levels/{levelState.GetLevelNumber()}") != null;

            return check1 && check2;
        }

        public void LoadLevel(int? levelNumber)
        {
            var ln = levelNumber ?? levelState.GetLevelNumber();
            state.Clear();
            levelState.SetLevelNumber(ln);

            ClearLastLevelData();

            LoadLevelConfig(ln);
            LoadLevelPrefab(ln);

            InitAllCells();

            cameraService.SetBoundingRect(levelState.GetRectExtra());
            cameraService.MoveTo(levelState.GetCenter(), true);
        }

        private void ClearLastLevelData()
        {
            levelState.Clear();
            destroyService.removeAllOnlyOnLevel();
        }

        private void LoadLevelConfig(int levelNumber)
        {
            var levelConfig = ResourcesUtils.LoadJson<LevelConfig>($"Levels/{levelNumber}");
            levelConfig.levelNumber = levelNumber;
            levelState.SetLevelConfig(ref levelConfig);
            
            levelState.SetLevelNumber(levelConfig.levelNumber);
            
            /* todo using player stats for increase started values */
            state.SetMaxLives(levelConfig.maxLives);
            state.SetLives(state.GetMaxLives());
            
            /* todo using player stats for increase started values */
            state.SetMaxEnergy(levelConfig.maxEnergy);
            state.SetEnergy(levelConfig.energy);
        }

        private void LoadLevelPrefab(int levelNumber)
        {
            if (levelGameObject != null)
            {
                Object.DestroyImmediate(levelGameObject);
            }

            levelGameObject = Object.Instantiate(
                Resources.Load<GameObject>($"Levels/{levelNumber}")
            );
        }

        private void InitAllCells()
        {
            foreach (var cell in Object.FindObjectsOfType<CellMonoBehaviour>())
            {
                levelState.PreAddCell(Cell.FromCellEditor(cell));
            }
            levelState.Build();
        }
    }
}