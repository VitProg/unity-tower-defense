using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy;
using td.features.eventBus;
using td.features.level.bus;
using td.features.level.cells;
using td.features.movement;
using td.features.path;
using td.features.pricePopup;
using td.features.shard.shardStore;
using td.features.state;
using td.features.tower;
using td.features.tower.mb;
using td.features.wave.bus;
using td.utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.level.systems
{
    public class Level_LoadingSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private LevelMap levelMap;
        [DI] private State state;
        [DI] private LevelLoader_Service levelLoader;
        [DI] private Path_Service pathService;
        [DI] private Enemy_Path_Service enemyPathService;
        [DI] private Tower_Converter towerConverter;
        [DI] private Tower_Service towerService;
        [DI] private Movement_Service movementService;
        [DI] private EventBus events;
        
        private ProtoWorld world;

        public void Init(IProtoSystems systems)
        {
            world = systems.World();
            events.unique.ListenTo<Command_LoadLevel>(OnLoadLevelCommand);
            events.unique.ListenTo<Event_LevelLoaded>(OnLevelLoaded);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Command_LoadLevel>(OnLoadLevelCommand);
            events.unique.RemoveListener<Event_LevelLoaded>(OnLevelLoaded);
        }
        
        //------------------------------------------//

        private void LoadLevel(ushort levelNumber)
        {
            state.SetLevelNumber(levelNumber);

            // todo: move to own modules
            state.Ex<ShardStore_StateEx>().SetVisible(false);
            state.Ex<PricePopup_StateExtension>().SetVisible(false);
            //

            if (levelLoader.HasLevel())
            {
                levelLoader.LoadLevel();
                pathService.InitPath();
                enemyPathService.PrecalculateAllPaths();

                state.Refresh();

                var levelConfig = levelMap.LevelConfig;

                var countdown = state.GetWaveNumber() <= 0
                    ? levelConfig?.delayBeforeFirstWave
                    : levelConfig?.delayBetweenWaves;

                Debug.Log(">>> Wave_NextCountdown" + (countdown ?? 0));
                Debug.Log(">>> Event_LevelLoaded");
                
                events.unique.GetOrAdd<Wave_NextCountdown>().countdown = countdown ?? 0;
                events.unique.GetOrAdd<Event_LevelLoaded>();
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
                var towerEntity = towerConverter.GetEntity(towerMb.gameObject) ?? world.NewEntity();
                towerConverter.Convert(towerMb.gameObject, towerEntity);

                var transform = movementService.GetGOTransform(towerEntity);

                var cellCoordinates = HexGridUtils.PositionToCell(transform.position);

                if (levelMap.HasCell(cellCoordinates, CellTypes.CanBuild)) {
                    // ToDo
                    ref var cell = ref levelMap.GetCell(cellCoordinates, CellTypes.CanBuild);
                    cell.packedBuildingEntity = world.PackEntityWithWorld(towerEntity);
                    cell.inputEventsHandlers.Add(towerService.GetTowerMB(towerEntity));
                    if (towerService.HasShardTower(towerEntity))
                    {
                        cell.inputEventsHandlers.Add(towerService.GetShardTowerMB(towerEntity));
                    }
                }
#if UNITY_EDITOR
                else
                {
                    throw new Exception($"Cell {cellCoordinates.x}, {cellCoordinates.y} is not type of CanBuild");
                }
#endif
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
            // Debug.Log("OnLoadLevelCommand begin");
                    
            // todo
            //systems.SetGroupSystemState(Constants.EcsSystemGroups.GameSimulation, true);
            // common.SetGroupSystemState(Constants.EcsSystemGroups.ShardSimulation, true);
            // common.SetGroupSystemState(Constants.EcsSystemGroups.Camera, true);
            // common.SetGroupSystemState(Constants.EcsSystemGroups.DragNDrop, true);
            // common.SetGroupSystemState(Constants.EcsSystemGroups.RemoveGameObject, true);
                    
            LoadLevel(command.levelNumber == default ? state.GetLevelNumber() : command.levelNumber);
                    
            // Debug.Log("OnLoadLevelCommand fin");
        }
    }
}