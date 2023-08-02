using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.enemy;
using td.features.level.bus;
using td.features.pathFinding;
using td.features.state;
using td.features.tower;
using td.features.tower.mb;
using td.features.wave.bus;
using td.monoBehaviours;
using td.utils;
using UnityEngine;

namespace td.features.level.systems
{
    public class Level_LoadingSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<IState> state;
        private readonly EcsInject<LevelLoader_Service> levelLoader;
        private readonly EcsInject<IPath_Service> pathService;
        private readonly EcsInject<EnemyPath_Service> enemyPathService;
        private readonly EcsInject<Tower_Converter> converter;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<SharedData> shared;
        private readonly EcsWorldInject world;

        public void Init(IEcsSystems systems)
        {
            events.Value.Unique.SubscribeTo<Command_LoadLevel>(OnLoadLevelCommand);
            events.Value.Unique.SubscribeTo<Event_LevelLoaded>(OnLevelLoaded);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Unique.RemoveListener<Command_LoadLevel>(OnLoadLevelCommand);
            events.Value.Unique.RemoveListener<Event_LevelLoaded>(OnLevelLoaded);
        }
        
        //------------------------------------------//

        private void LoadLevel(ushort levelNumber)
        {
            state.Value.LevelNumber = levelNumber;

            state.Value.ShardStore.Visible = false;
            state.Value.CostPopup.Visible = false;

            if (levelLoader.Value.HasLevel())
            {
                levelLoader.Value.LoadLevel();
                pathService.Value.InitPath();
                enemyPathService.Value.PrecalculateAllPaths();

                state.Value.Refresh();

                var levelConfig = levelMap.Value.LevelConfig;

                var countdown = state.Value.WaveNumber <= 0
                    ? levelConfig?.delayBeforeFirstWave
                    : levelConfig?.delayBetweenWaves;

                events.Value.Unique.Add<Wave_NextCountdown>().countdown = countdown ?? 0;
                events.Value.Unique.Add<Event_LevelLoaded>();
            }
            else
            {
                Debug.Log("ALL LEVELS ARE FINISHED!");
            }
        }

        private void InitBuildings()
        {
            foreach (var towerMb in Object.FindObjectsOfType<TowerMonoBehaviour>())
            {
                var entity = converter.Value.GetEntity(towerMb.gameObject) ?? world.Value.NewEntity();
                converter.Value.Convert(towerMb.gameObject, entity);

                var transform = common.Value.GetGOTransform(entity);

                var cellCoordinates = HexGridUtils.PositionToCell(transform.position);

                if (levelMap.Value.HasCell(cellCoordinates, CellTypes.CanBuild)) {
                    // ToDo
                    levelMap.Value.GetCell(cellCoordinates, CellTypes.CanBuild).packedBuildingEntity = world.Value.PackEntity(entity);
                }
                else
                {
                    throw new System.Exception($"Cell {cellCoordinates.x}, {cellCoordinates.y} is not type of CanBuild");
                }
            }
        }

        private void OnLevelLoaded(ref Event_LevelLoaded @event)
        {
            // Debug.Log("OnLevelLoaded begin");
            InitBuildings();
            // Debug.Log("OnLevelLoaded fin");
        }

        private void OnLoadLevelCommand(ref Command_LoadLevel command)
        {
            Debug.Log("OnLoadLevelCommand begin");
                    
            // todo
            //systems.SetGroupSystemState(Constants.EcsSystemGroups.GameSimulation, true);
            common.Value.SetGroupSystemState(Constants.EcsSystemGroups.ShardSimulation, true);
            common.Value.SetGroupSystemState(Constants.EcsSystemGroups.Camera, true);
            common.Value.SetGroupSystemState(Constants.EcsSystemGroups.DragNDrop, true);
            common.Value.SetGroupSystemState(Constants.EcsSystemGroups.RemoveGameObject, true);
                    
            LoadLevel(command.levelNumber);
                    
            Debug.Log("OnLoadLevelCommand fin");
        }
    }
}