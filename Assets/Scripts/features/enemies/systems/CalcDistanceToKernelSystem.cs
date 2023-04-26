using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.components.refs;
using td.features.enemies.components;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies.systems
{
    public class CalcDistanceToKernelSystem : IEcsRunSystem
    {
        [Inject] private LevelMap levelMap;

        private readonly EcsFilterInject<Inc<Enemy, Ref<GameObject>>, Exc<IsEnemyDead, IsDestroyed>> entities = default;
            
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var enemy = ref entities.Pools.Inc1.Get(entity);
                var enemyGameObject = entities.Pools.Inc2.Get(entity);
                var enemyPosition = enemyGameObject.reference.transform.position;
                var enemyCoordinate = HexGridUtils.PositionToCell(enemyPosition);

                // todo считать все альтернативные маршруты??
                if (
                    levelMap.TryGetCell(enemyCoordinate, out var cell) &&
                    !cell.isKernel &&
                    levelMap.TryGetCell(cell.NextCoords, out var nextCell)
                ) {
                    var numberOfCellsToKernel = cell.distanceToKernel;
                    var nextCellPosition = HexGridUtils.CellToPosition(nextCell.Coords);

                    var distanceToKernel = 
                        (numberOfCellsToKernel - 1) +
                        (enemyPosition - (Vector3)nextCellPosition).magnitude;

                    enemy.distanceToKernel = distanceToKernel;
                    // enemy.distanceFromSpawn = cell.distanceFromSpawn; //todo
                }
                else
                {
                    enemy.distanceToKernel = 0f;
                    enemy.distanceFromSpawn = 999f;
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