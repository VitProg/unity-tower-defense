using Leopotam.EcsLite;
using td.common;
using td.common.level;
using td.features.enemies;
using td.utils.ecs;
using Unity.Mathematics;
using UnityEngine;

namespace td.utils
{
    public static class EnemyUtils
    {
        public static Vector2 TargetPosition(Int2 cellCoordinates, Quaternion rotation, Vector2 offset, LevelCellType cellType, float cellSize) =>
            GridUtils.CellToCoords(cellCoordinates, cellType, cellSize) +  (Vector2)(rotation * offset);

        public static Quaternion LookToNextCell(Int2 currentCellCoordinates, Int2 nextCellCoordinates, LevelCellType cellType, float cellSize)
        {
            var toNextCellVector = GridUtils.CellToCoords(nextCellCoordinates, cellType, cellSize) -
                                   GridUtils.CellToCoords(currentCellCoordinates, cellType, cellSize);
            toNextCellVector.Normalize();
            
            return Quaternion.LookRotation(Vector3.forward, toNextCellVector);
        }

        public static float GetAngularSpeed(EcsWorld world, int ennemyEntity)
        {
            var angularSpeed = Constants.Enemy.DefaultAngularSpeed;
                        
            if (world.HasComponent<Enemy>(ennemyEntity))
            {
                ref var enemy = ref world.GetComponent<Enemy>(ennemyEntity);
                angularSpeed = enemy.angularSpeed > Constants.Enemy.MinAngularSpeed 
                    ? enemy.angularSpeed
                    : Constants.Enemy.DefaultAngularSpeed;
            }

            return angularSpeed;
        }

        public static int GetEnemiesCount(EcsWorld world) => world.Filter<Enemy>().Exc<IsEnemyDead>().End().GetEntitiesCount();
    }
}