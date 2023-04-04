using Leopotam.EcsLite;
using td.common;
using td.features.enemies;
using td.utils.ecs;
using Unity.Mathematics;
using UnityEngine;

namespace td.utils
{
    public static class EnemyUtils
    {
        public static Vector2 TargetPosition(Int2 cellCoordinates, Quaternion rotation, Vector2 offset) =>
            GridUtils.GetVector(cellCoordinates) +
            (Vector2)(rotation * offset);

        public static Quaternion LookToNextCell(Int2 currentCellCoordinates, Int2 nextCellCoordinates)
        {
            var toNextCellVector = GridUtils.GetVector(nextCellCoordinates) -
                                   GridUtils.GetVector(currentCellCoordinates);
            toNextCellVector.Normalize();
            
            return Quaternion.LookRotation(Vector3.forward, toNextCellVector);
        }

        public static float GetAngularSpeed(IEcsSystems systems, int ennemyEntity)
        {
            var angularSpeed = Constants.Enemy.DefaultAngularSpeed;
                        
            if (EntityUtils.HasComponent<SpawnEnemyCommand>(systems, ennemyEntity))
            {
                ref var spawConfig = ref EntityUtils.GetComponent<SpawnEnemyCommand>(systems, ennemyEntity);
                angularSpeed = spawConfig.angularSpeed > Constants.Enemy.MinAngularSpeed 
                    ? spawConfig.angularSpeed
                    : Constants.Enemy.DefaultAngularSpeed;
            }

            return angularSpeed;
        }
    }
}