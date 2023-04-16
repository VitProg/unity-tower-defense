using System;
using Leopotam.EcsLite;
using td.common;
using td.common.cells;
using td.common.cells.hex;
using td.common.cells.interfaces;
using td.common.level;
using td.components;
using td.components.flags;
using td.features.towers;
using td.monoBehaviours;
using td.states;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.services
{
    public class LevelLoader
    {
        private readonly LevelMap levelMap;
        private readonly LevelState levelState;

        private GameObject levelGameObject;

        public LevelLoader(LevelMap levelMap, LevelState levelState)
        {
            this.levelMap = levelMap;
            this.levelState = levelState;
        }

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

                InitCanWalkTiles();
                InitCanBuildTiles();
                InitSpawns();
                InitKernels();

                // InitBuildings(world);

                InitGridRenderer();

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
            levelMap.LevelConfig = ResourcesUtils.LoadJson<LevelConfig>($"Levels/{levelState.LevelNumber}");
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

        private void InitCanWalkTiles()
        {
            float? cellSize = null;

            var tiles = GameObject.FindGameObjectsWithTag(Constants.Tags.CanWalk);
            foreach (var tileGameObject in tiles)
            {
                if (cellSize == null)
                {
                    var sx = tileGameObject.transform.localScale.x;
                    cellSize = (Mathf.Round(sx * 10f) / 10f);
                    levelMap.CellSize = cellSize.Value;
                }

                var coordinates = GridUtils.CoordsToCell(
                    tileGameObject.transform.position,
                    levelMap.CellType,
                    levelMap.CellSize
                );

                if (levelMap.CellType == LevelCellType.Hex)
                {
                    var cell = new HexCellCanWalk()
                    {
                        Coordinates = coordinates,
                        GameObject = tileGameObject,
                    };
                    levelMap.AddCell(cell);
                }
                else
                {
                    var cell = new CellCanWalk()
                    {
                        Coordinates = coordinates,
                        GameObject = tileGameObject,
                    };
                    levelMap.AddCell(cell);
                }
            }
        }

        private void InitCanBuildTiles()
        {
            var tiles = GameObject.FindGameObjectsWithTag(Constants.Tags.CanBuild);
            foreach (var tileGameObject in tiles)
            {
                var coordinates = GridUtils.CoordsToCell(
                    tileGameObject.transform.position,
                    levelMap.CellType,
                    levelMap.CellSize
                );

                if (levelMap.CellType == LevelCellType.Hex)
                {
                    var cell = new HexCellCanBuild()
                    {
                        Coordinates = coordinates,
                        // GameObject = tileGameObject,
                    };
                    levelMap.AddCell(cell);
                }
                else
                {
                    var cell = new CellCanBuild()
                    {
                        Coordinates = coordinates,
                        // GameObject = tileGameObject,
                    };
                    levelMap.AddCell(cell);
                }
            }
        }

        private void InitSpawns()
        {
            var spawns = GameObject.FindGameObjectsWithTag(Constants.Tags.Spawn);
            foreach (var spawnGameObject in spawns)
            {
                var spawn = new Spawn()
                {
                    Coordinates = GridUtils.CoordsToCell(
                        spawnGameObject.transform.position,
                        levelMap.LevelConfig?.cellType ?? LevelCellType.Square,
                        levelMap.CellSize
                    ),
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
                    Coordinates = GridUtils.CoordsToCell(
                        targetGameObject.transform.position,
                        levelMap.CellType,
                        levelMap.CellSize
                    ),
                };
                levelMap.AddKernel(kernel);
            }
        }

        public void InitBuildings(EcsWorld world)
        {
            var towerPool = world.GetPool<Tower>();
            var goPool = world.GetPool<Ref<GameObject>>();
            // var entities = world.Filter<Tower>().Inc<Ref<GameObject>>().End();
            // foreach (var entity in entities)

            foreach (var toerTransform in Object.FindObjectsOfType<TowerProvider>())
            {
                var entity = world.ConvertToEntity(toerTransform.gameObject);

                var tower = towerPool.Get(entity);
                var towerGameObject = goPool.Get(entity);

                var cellCoordinates = GridUtils.CoordsToCell(
                    towerGameObject.reference.transform.position,
                    levelMap.CellType,
                    levelMap.CellSize
                );

                var cell = levelMap.GetCell<ICellCanBuild>(cellCoordinates);

                if (cell != null)
                {
                    cell.BuildingPackedEntity = world.PackEntity(entity);
                }

                if (tower.radiusGameObject == null)
                {
                    var radiusTransform = towerGameObject.reference.transform.Find("radius");
                    if (radiusTransform != null)
                    {
                        tower.radiusGameObject = radiusTransform.gameObject;
                        tower.radiusGameObject.SetActive(false);
                    }
                }

                // var radius = towerGameObject.reference.transform.Find("radius");
                // radius.gameObject.hideFlags = HideFlags.None;
                // radius.localScale = new Vector3(tower.radius, tower.radius, tower.radius) * 1.3f;
            }
        }

        private void InitGridRenderer()
        {
            levelMap.GridRenderer = null;
            var go = GameObject.FindGameObjectWithTag(Constants.Tags.GridRenderer);
            if (go)
            {
                var hl = go.GetComponent<HightlightGridByCursor>();
                levelMap.GridRenderer = hl;
            }
        }
    }
}