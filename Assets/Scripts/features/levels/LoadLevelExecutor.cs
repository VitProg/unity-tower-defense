using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.features.enemies;
using td.features.enemies.components;
using td.features.towers;
using td.features.towers.mb;
using td.features.ui;
using td.features.waves;
using td.monoBehaviours;
using td.services;
using td.services.ecsConverter;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.levels
{
    public class LoadLevelExecutor : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;
        
        [Inject] private LevelMap levelMap;
        [Inject] private LevelState levelState;
        [Inject] private LevelLoader levelLoader;
        [Inject] private IPathService pathService;
        [Inject] private EnemyPathService enemyPathService;
        [Inject] private EntityConverters converters;
        [InjectShared] private SharedData shared;

        private readonly EcsFilterInject<Inc<LoadLevelOuterCommand>> loadCommandEntities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<LevelLoadedOuterEvent>> loadedEventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            try
            {
                foreach (var entity in loadCommandEntities.Value)
                {
                    Load(systems, entity);
                    systems.DelOuter<LoadLevelOuterCommand>();
                    break;
                }

                foreach (var entity in loadedEventEntities.Value)
                {
                    InitBuildings();
                    InitShardsPanel();
                    InitBuyShardPopup();
                    systems.DelOuter<LevelLoadedOuterEvent>();
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void InitBuildings()
        {
            var towerPool = world.GetPool<Tower>();
            var goPool = world.GetPool<Ref<GameObject>>();

            foreach (var towerMB in Object.FindObjectsOfType<TowerMonoBehaviour>())
            {
                if (!converters.Convert<Tower>(towerMB.gameObject, out var entity))
                {
                    throw new NullReferenceException($"Failed to convert GameObject {towerMB.gameObject.name}");
                }

                var tower = towerPool.Get(entity);
                var towerGameObject = goPool.Get(entity);

                var cellCoordinates = HexGridUtils.PositionToCell(towerGameObject.reference.transform.position);

                var cell = levelMap.GetCell(cellCoordinates, CellTypes.CanBuild);

                if (cell != null)
                {
                    // ToDo
                    cell.buildings[0] = world.PackEntity(entity);
                }

                // if (tower.radiusGameObject == null)
                // {
                //     var radiusTransform = towerGameObject.reference.transform.Find("outerRadius");
                //     if (radiusTransform != null)
                //     {
                //         tower.radiusGameObject = radiusTransform.gameObject;
                //         tower.radiusGameObject.SetActive(false);
                //     }
                // }
            }
        }

        private void InitShardsPanel()
        {
            if (shared.shardsPanel == null) return;
            shared.shardsPanel.Refresh();
        }

        private void InitBuyShardPopup()
        {
            if (shared.buyShardPopup == null) return;
            shared.buyShardPopup.Refresh();
        }

        private void Load(IEcsSystems systems, int entity)
        {
            var levelNumber = loadCommandEntities.Pools.Inc1.Get(entity).levelNumber;
            levelState.LevelNumber = levelNumber;

            if (levelLoader.HasLevel())
            {
                levelLoader.LoadLevel(systems);
                pathService.InitPath();
                enemyPathService.PrecalculateAllPaths();

                systems.DelOuter<IsLoadingOuter>();
                ref var updateUI = ref systems.OuterSingle<UpdateUIOuterCommand>();
                updateUI.lives = levelState.Lives;
                updateUI.maxLives = levelState.MaxLives;
                updateUI.money = levelState.Money;
                updateUI.levelNumber = levelState.LevelNumber;
                updateUI.enemiesCount = levelState.EnemiesCount;
                updateUI.isLastWave = levelState.IsLastWave;
                updateUI.nextWaveCountdown = levelState.NextWaveCountdown;
                updateUI.levelNumber = (uint)levelState.WaveNumber;
                updateUI.waveCount = levelState.WaveCount;

                var countdown = levelState.WaveNumber <= 0
                    ? levelMap.LevelConfig?.delayBeforeFirstWave
                    : levelMap.LevelConfig?.delayBetweenWaves;

                systems.OuterSingle<NextWaveCountdownOuter>().countdown = countdown ?? 0;

                systems.Outer<LevelLoadedOuterEvent>();
            }
            else
            {
                Debug.Log("ALL LEVELS ARE FINISHED!");
            }
        }
    }
}