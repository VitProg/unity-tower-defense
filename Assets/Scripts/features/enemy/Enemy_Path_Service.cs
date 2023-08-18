using System.Collections.Generic;
using Leopotam.EcsProto.QoL;
using td.features.enemy.components;
using td.features.level;
using td.features.level.cells;
using td.utils;
using td.utils.ecs;
using Unity.Mathematics;

namespace td.features.enemy
{
    public class Enemy_Path_Service
    {
        [DI] private Enemy_Aspect aspect;
        [DI] private LevelMap levelMap;

        // private readonly Dictionary<string, List<byte>> cache = new (1);
        private Dictionary<string, List<List<int2>>> allPathsCache = new ();

        public void PrecalculateAllPaths()
        {
            allPathsCache.Clear();
            allPathsCache = new Dictionary<string, List<List<int2>>>();

            for (var spawnIndex = 0; spawnIndex < levelMap.SpawnCount; spawnIndex++)
            {
                var spawnCoords = levelMap.spawns[spawnIndex];
                
                if (!spawnCoords.HasValue) return;

                var sCache = new List<List<int2>>();
                
                if (!levelMap.HasCell(spawnCoords, CellTypes.CanWalk)) continue;
                
                ref var cell = ref levelMap.GetCell(spawnCoords.Value, CellTypes.CanWalk);
                
                var path = new List<int2> { spawnCoords.Value };
                sCache.Add(path);
                var nextCoords = HexGridUtils.GetNeighborsCoords(ref cell.coords, cell.dirToNext);
                Tick(ref nextCoords, ref path, ref sCache);

                var sCacheFiltered = new List<List<int2>>(sCache.Count / 2);

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

        private void Tick(ref int2 coords, ref List<int2> path, ref List<List<int2>> sCache, int depth = 0)
        {
            if (!levelMap.HasCell(coords.x, coords.y, CellTypes.CanWalk)) return; 
            ref var cell = ref levelMap.GetCell(coords.x, coords.y, CellTypes.CanWalk);

            var n = 0; //path.FindAll(i => i.x == x && i.y == y).Count;
            foreach (var pathItem in path)
            {
                n += pathItem.x == coords.x && pathItem.y == coords.y ? 1 : 0;
            }

            // если мы уже проходили эту клетку 1 раз, то удаляем этот маршрут
            if (n > 1 || depth > 1000)
            {
                path.Clear();
                return;
            }

            path.Add(new int2(coords.x, coords.y));
            
            if (cell.isKernel) return;
            
            if (cell is { isSwitcher: true, HasNextAltDir: true })
            {
                var altPath = new List<int2>(path);
                sCache.Add(altPath);
                var nextAltCoords = HexGridUtils.GetNeighborsCoords(ref cell.coords, cell.dirToNextAlt);
                Tick(ref nextAltCoords, ref altPath, ref sCache, depth + 1);
            }
            
            if (cell.HasNextDir) { 
                var nextCoords = HexGridUtils.GetNeighborsCoords(ref cell.coords, cell.dirToNext);
                Tick(ref nextCoords, ref path, ref sCache, depth + 1);
            }
        }


        public int RandomPathNumber(ref int2 spawnCoords)
        {
            var spawnKey = spawnCoords.ToString();
            var currentCache = allPathsCache[spawnKey];
            return currentCache.Count == 1 ? 0 : RandomUtils.IntRange(0, currentCache.Count - 1);
        }
        public void PrepareEnemyPath(ref int2 spawnCoords, int enemyEntity)
        {
            ref var enemyPath = ref aspect.enemyPathPool.GetOrAdd(enemyEntity);

            enemyPath.spawnKey = spawnCoords.ToString();

            var currentCache = allPathsCache[enemyPath.spawnKey];

            var randomIndex = currentCache.Count == 1 ? 0 : RandomUtils.IntRange(0, currentCache.Count - 1);
            
            enemyPath.pathNumber = randomIndex;
            enemyPath.index = 0;
        }

        public void SetPath(int enemyEntity, ref int2 spawnCoords, int pathNumber)
        {
            var spawnKey = spawnCoords.ToString();
            ref var enemyPath = ref aspect.enemyPathPool.GetOrAdd(enemyEntity);
            enemyPath.spawnKey = spawnKey;
            enemyPath.pathNumber = pathNumber;
            enemyPath.index = 0;
        }

        public List<int2> GetPath(ref Enemy_Path enemyPath)
        {
            return allPathsCache[enemyPath.spawnKey][enemyPath.pathNumber];
        }       
        
        public List<int2> GetPath(ref int2 spawnCoords, int pathNumber)
        {
            var spawnKey = spawnCoords.ToString();
            return allPathsCache[spawnKey][pathNumber];
        }
        
        public List<int2> GetPath(int enemyEntity)
        {
            ref var enemyPath = ref aspect.enemyPathPool.GetOrAdd(enemyEntity);
            return GetPath(ref enemyPath);
        }
    }
}
