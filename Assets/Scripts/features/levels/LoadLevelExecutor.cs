using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.features.enemies;
using td.features.shards;
using td.features.state;
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
        [Inject] private LevelMap levelMap;
        [Inject] private State state;
        [Inject] private LevelLoader levelLoader;
        [Inject] private IPathService pathService;
        [Inject] private EnemyPathService enemyPathService;
        [Inject] private EntityConverters converters;
        [InjectWorld] private EcsWorld world;
        [InjectSystems] private IEcsSystems systems;
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
                    //systems.DelOuter<LevelLoadedOuterEvent>();
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

            foreach (var towerMb in Object.FindObjectsOfType<TowerMonoBehaviour>())
            {
                if (!converters.Convert<Tower>(towerMb.gameObject, out var entity))
                {
                    throw new NullReferenceException($"Failed to convert GameObject {towerMb.gameObject.name}");
                }

                var tower = towerPool.Get(entity);
                var towerGameObject = goPool.Get(entity);

                var cellCoordinates = HexGridUtils.PositionToCell(towerGameObject.reference.transform.position);

                var cell = levelMap.GetCell(cellCoordinates, CellTypes.CanBuild);

                if (cell != null)
                {
                    // ToDo
                    cell.buildingPackedEntity = world.PackEntity(entity);
                }
            }
        }

        private void Load(IEcsSystems systems, int entity)
        {
            var levelNumber = loadCommandEntities.Pools.Inc1.Get(entity).levelNumber;
            state.LevelNumber = levelNumber;

            if (levelLoader.HasLevel())
            {
                levelLoader.LoadLevel(systems);
                pathService.InitPath();
                enemyPathService.PrecalculateAllPaths();

                systems.DelOuter<IsLoadingOuter>();
                state.Refresh();

                var countdown = state.WaveNumber <= 0
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