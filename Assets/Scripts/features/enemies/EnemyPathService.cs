using System.Collections.Generic;
using Leopotam.EcsLite;
using td.common;
using td.features.enemies.components;
using td.monoBehaviours;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies
{
    public class EnemyPathService
    {
        [Inject] private LevelMap levelMap;
        [InjectWorld] private EcsWorld world;

        private readonly Dictionary<string, List<byte>> cache = new (1);
        private Dictionary<string, List<List<Int2>>> allPathsCache = new ();

        public void PrecalculateAllPaths()
        {
            allPathsCache = new Dictionary<string, List<List<Int2>>>();
            
            foreach (var spawnCoords in levelMap.Spawns)
            {
                var sCache = new List<List<Int2>>();
                
                var cell = levelMap.GetCell(spawnCoords, CellTypes.CanWalk);
                
                if (!cell || !cell.NextCoords.HasValue) continue;
                
                var path = new List<Int2>();
                sCache.Add(path);
                Tick(cell.NextCoords.Value, path, sCache);

                var sCacheFiltered = new List<List<Int2>>(sCache.Count / 2);

                foreach (var pathItem in sCache)
                {
                    if (pathItem.Count > 0)
                    {
                        sCacheFiltered.Add(pathItem);
                    }
                }

                allPathsCache.Add(spawnCoords.ToString(), sCacheFiltered);
            }
        }

        private void Tick(Int2 coords, List<Int2> path, List<List<Int2>> sCache, int depth = 0)
        {
            var cell = levelMap.GetCell(coords, CellTypes.CanWalk);
            
            if (!cell) return;

            // var direction = HexGridUtils.GetDirection(coords, coords);

            var n = path.FindAll(i => i == coords).Count;

            // если мы уже проходили эту клетку 1 раз, то удаляем этот маршрут
            if (n > 1 || depth > 1000)
            {
                path.Clear();
                return;
            }
            
            path.Add(coords);
            
            if (cell.isKernel) return;
            
            if (cell.isSwitcher && cell.AltNextCoords.HasValue)
            {
                var altPath = new List<Int2>(path);
                sCache.Add(altPath);
                Tick(cell.AltNextCoords.Value, altPath, sCache, depth + 1);
            }
            
            if (cell.NextCoords.HasValue) { 
                Tick(cell.NextCoords.Value, path, sCache, depth + 1);
            }
        }


        public void PrepareEnemyPath(ref Int2 spawnCoords, int enemyEntity)
        {
            ref var enemyPath = ref world.GetComponent<EnemyPath>(enemyEntity);

            enemyPath.spawnKey = spawnCoords.ToString();

            var currentCache = allPathsCache[enemyPath.spawnKey];

            var randomIndex = currentCache.Count == 1 ? 0 : RandomUtils.IntRange(0, currentCache.Count - 1);
            if (randomIndex == 1)
            {
                Debug.DebugBreak();
            }

            enemyPath.pathNumber = randomIndex;
            enemyPath.index = 0;
        }

        public List<Int2> GetPath(ref EnemyPath enemyPath)
        {
            return allPathsCache[enemyPath.spawnKey][enemyPath.pathNumber];
        }
        
        public List<Int2> GetPath(int enemyEntity)
        {
            ref var enemyPath = ref world.GetComponent<EnemyPath>(enemyEntity);
            return GetPath(ref enemyPath);
        }
    }
}
