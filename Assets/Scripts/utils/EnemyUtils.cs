using Leopotam.EcsLite;
using td.common;
using td.features.enemies;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;

namespace td.utils
{
    public static class EnemyUtils
    {
        public static Vector2 Position(Int2 cellCoordinates, Quaternion rotation, Vector2 offset) =>
            HexGridUtils.CellToPosition(cellCoordinates) +  (Vector2)(rotation * offset);

        public static Quaternion LookToNextCell(Cell currentCell, Cell nextCell)
        {
            var currentCellCoordinates = currentCell.Coords;
            var nextCellCoordinates = nextCell.Coords;

            var toNextCellVector = HexGridUtils.CellToPosition(nextCellCoordinates) -
                                   HexGridUtils.CellToPosition(currentCellCoordinates);
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