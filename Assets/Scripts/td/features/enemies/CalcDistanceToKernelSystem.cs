using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.links;
using td.services;
using td.utils;
using UnityEngine;

namespace td.features.enemies
{
    public class CalcDistanceToKernelSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<IsEnemy, GameObjectLink>, Exc<IsEnemyDead>> entities = default;
            
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var enemy = ref entities.Pools.Inc1.Get(entity);
                var enemyGameObject = entities.Pools.Inc2.Get(entity);
                var enemyPosition = enemyGameObject.gameObject.transform.position;
                var enemyCoordinate = GridUtils.GetGridCoordinate(enemyPosition);
                var cell = levelData.Value.GetCell(enemyCoordinate);
                
                if (!cell.isKernel)
                {
                    var nextCell = levelData.Value.GetCell(cell.NextCellCoordinates);
                    var numberOfCellsToKernel = cell.distanceToKernel;
                    var nextCellPosition = GridUtils.GetVector(nextCell.Coordinates);

                    var distanceToKernel = 
                        (numberOfCellsToKernel - 1) * Constants.Level.CellSize +
                        (enemyPosition - (Vector3)nextCellPosition).magnitude;

                    enemy.distanceToKernel = distanceToKernel;
                }
                else
                {
                    enemy.distanceToKernel = 0f;
                }
            }
        }
    }

    // internal struct Datum
    // {
    //     public int NumberOfCellsToKernel;
    //     public Int2 nextCellCoordinate;
    // }
    //
    // [BurstCompile]
    // internal struct CalcDistanceToKernelJob : IJobParallelFor
    // {
    //     [NativeDisableParallelForRestriction]
    //     public NativeArray<Datum> data;
    //
    //     [NativeDisableParallelForRestriction]
    //     public NativeArray<double> result;
    //
    //     public void Execute(int index)
    //     {
    //         var datum = data[index];
    //         
    //         
    //     }
    // }
}