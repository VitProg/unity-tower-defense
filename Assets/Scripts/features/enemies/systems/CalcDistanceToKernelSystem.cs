using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.behaviors;
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
        [Inject] private EnemyPathService enemyPathService;

        private readonly EcsFilterInject<Inc<Enemy, Ref<GameObject>, EnemyPath, LinearMovementToTarget>, Exc<IsEnemyDead, IsDestroyed>> entities = default;
            
        public void Run(IEcsSystems systems)
        {
            foreach (var enemyEntity in entities.Value)
            {
                ref var enemy = ref entities.Pools.Inc1.Get(enemyEntity);
                ref var enemyPath = ref entities.Pools.Inc3.Get(enemyEntity);;
                ref var toTarget = ref entities.Pools.Inc4.Get(enemyEntity);;
                
                var enemyGameObject = entities.Pools.Inc2.Get(enemyEntity);
                var enemyPosition = enemyGameObject.reference.transform.position;

                var path = enemyPathService.GetPath(ref enemyPath);

                var step = path[enemyPath.index];
                var nextStep = enemyPath.index + 1 < path.Count ? path[enemyPath.index + 1] : (Int2?)null;

                var nextCellPosition = nextStep.HasValue
                    ? EnemyUtils.Position(nextStep.Value, enemyGameObject.reference.transform.rotation, enemy.offset) : 
                    (Vector2?)null;
                
                var percentToNextCell = Mathf.Min(1f, nextCellPosition.HasValue ? ((Vector2)enemyPosition - toTarget.target).magnitude : 0f);

                var numberOfCellsToKernel = path.Count - enemyPath.index;
                
                var distanceToKernel = numberOfCellsToKernel + percentToNextCell - 1f;
                
                enemy.distanceToKernel = distanceToKernel;
                //
                // // todo считать все альтернативные маршруты??
                // if (
                //     levelMap.TryGetCell(enemyCoordinate, out var cell) &&
                //     !cell.isKernel &&
                //     levelMap.TryGetCell(cell.NextCoords, out var nextCell)
                // ) {
                //     var numberOfCellsToKernel = cell.distanceToKernel;
                //     var nextCellPosition = HexGridUtils.CellToPosition(nextCell.Coords);
                //
                //     var distanceToKernel = 
                //         (numberOfCellsToKernel - 1) +
                //         (enemyPosition - (Vector3)nextCellPosition).magnitude;
                //
                //     enemy.distanceToKernel = distanceToKernel;
                //     // enemy.distanceFromSpawn = cell.distanceFromSpawn; //todo
                // }
                // else
                // {
                //     enemy.distanceToKernel = 0f;
                //     enemy.distanceFromSpawn = 999f;
                // }
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