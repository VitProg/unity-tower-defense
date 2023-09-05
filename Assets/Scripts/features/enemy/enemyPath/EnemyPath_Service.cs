using Leopotam.EcsProto.QoL;
using td.features.enemy.components;
using td.features.level;
using td.features.level.cells;
using td.utils;
using td.utils.ecs;
using Unity.Mathematics;

namespace td.features.enemy.enemyPath {
    public class EnemyPath_Service {
        [DI] private Enemy_Aspect aspect;
        [DI] private Level_State levelState;
        [DI] private EnemyPath_State enemyPathState;

        public void PrecalculateAllPaths() {
            enemyPathState.Clear();

            for (var spawnIndex = 0; spawnIndex < levelState.GetSpawnCount(); spawnIndex++) {
                ref var spawnCell = ref levelState.GetSpawnByIndex(spawnIndex);
                if (!spawnCell.isSpawn || spawnCell.type != CellTypes.CanWalk) continue;

                var routeIdx = enemyPathState.NewRoute();
                enemyPathState.AddRouteItem(routeIdx, spawnCell.coords.x, spawnCell.coords.y);
                
                var nextCoords = HexGridUtils.GetNeighborsCoords(ref spawnCell.coords, spawnCell.dirToNext);
                
                Tick(ref nextCoords, routeIdx);
            }
            enemyPathState.ClearEmptyRoutes();
        }

        private void Tick(ref int2 coords, int routeIdx, int depth = 0) {
            if (!levelState.HasCell(coords.x, coords.y, CellTypes.CanWalk)) return;

            // если мы уже проходили эту клетку 1 раз, то удаляем этот маршрут
            if (enemyPathState.RouteHaveCoord(routeIdx, coords.x, coords.y) || depth > 500) {
                enemyPathState.DeleteRoute(routeIdx);
                return;
            }
            
            ref var cell = ref levelState.GetCell(coords.x, coords.y, CellTypes.CanWalk);

            enemyPathState.AddRouteItem(routeIdx, coords.x, coords.y);

            if (cell.isKernel) return;

            if (cell is { isSwitcher: true, HasNextAltDir: true }) {
                var altRouteIdx = enemyPathState.CopyRoute(routeIdx);
                var nextAltCoords = HexGridUtils.GetNeighborsCoords(ref cell.coords, cell.dirToNextAlt);
                Tick(ref nextAltCoords, altRouteIdx, depth + 1);
            }

            if (cell.HasNextDir) {
                var nextCoords = HexGridUtils.GetNeighborsCoords(ref cell.coords, cell.dirToNext);
                Tick(ref nextCoords, routeIdx, depth + 1);
            }
        }

        private int[] findedRoutes = new int[16];

        public int RandomPathNumber(ref int2 spawnCoords) {
            if (!enemyPathState.RoutesByFirstCoord(spawnCoords.x, spawnCoords.y, ref findedRoutes, out var count)) return -1;
            return findedRoutes[RandomUtils.IntRange(0, count - 1)];
        }

        public void PrepareEnemyPath(ref int2 spawnCoords, int enemyEntity) {
            ref var enemyPath = ref aspect.enemyRoutePool.GetOrAdd(enemyEntity);
            var routeIdx = RandomPathNumber(ref spawnCoords);
            
            enemyPath.routeIdx = routeIdx;
            enemyPath.step = 0;
        }

        public void SetRoute(int enemyEntity, int routeIdx) {
            ref var enemyPath = ref aspect.enemyRoutePool.GetOrAdd(enemyEntity);
            enemyPath.routeIdx = routeIdx;
            enemyPath.step = 0;
        }

        public ref Enemy_Route GetRoute(int enemyEntity) => ref aspect.enemyRoutePool.GetOrAdd(enemyEntity);
    }
}
