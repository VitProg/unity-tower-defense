using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.features.waves;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.levels
{
    public class PathInitExecutor : IEcsRunSystem
    {
        private readonly Int2 toLeft = new Int2(-1, 0);
        private readonly Int2 toRight = new Int2(1, 0);
        private readonly Int2 toTop = new Int2(0, 1);
        private readonly Int2 toBottom = new Int2(0, -1);
        
        private readonly Queue<Cell> queue = new Queue<Cell>();
        
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<PathInitCommand>> entities = Constants.Ecs.EventsWorldName;

        public void Run(IEcsSystems systems)
        {
            var entity = EcsEventUtils.FirstEntity(entities);
            
            if (entity == null) return;
            
            Debug.Log("PathInitExecutor RUN...");
            
            queue.Clear();
            
            Debug.Assert(levelData.Value.Kernels != null);
            var kernels = levelData.Value.Kernels;
            
            foreach (var kernel in kernels)
            {
                var kernelCell = levelData.Value.GetCell(kernel.Coordinates);
                Debug.Assert(kernelCell != null);
                kernelCell.isKernel = true;
                kernelCell.distanceToKernel = 0;
                queue.Enqueue(kernelCell);
            }

            do
            {
                var cell = queue.Dequeue();
                Tick(cell);
            } while (queue.Count > 0);

            EcsEventUtils.Send<LevelLoadedEvent>(systems);
            
            queue.Clear();
                
            EcsEventUtils.CleanupEvent(systems, entities);
            
            Debug.Log("PathInitExecutor FIN");
        }

        private void Tick(Cell cell)
        {
            var even = cell.Coordinates.x % 2 == 0;
            
            var nearestCells = new Cell[4]
            {
                levelData.Value.GetCell(cell.Coordinates - (even ? toLeft : toTop)),
                levelData.Value.GetCell(cell.Coordinates - (even ? toBottom : toLeft)),
                levelData.Value.GetCell(cell.Coordinates - (even ? toRight : toBottom)),
                levelData.Value.GetCell(cell.Coordinates - (even ? toTop : toRight)),
            };

            foreach (var nearestCell in nearestCells)
            { 
                if (nearestCell == null || !nearestCell.NextCellCoordinates.empty) continue;
                    
                nearestCell.NextCellCoordinates = cell.Coordinates;
                nearestCell.distanceToKernel = cell.distanceToKernel + 1;
                
                var arrow = nearestCell.gameObject.transform.Find("arrow");
                
#if UNITY_EDITOR
                if (arrow && !nearestCell.isKernel)
                {
                    var vector = cell.Coordinates - nearestCell.Coordinates;
                    if (!vector.empty)
                    {
                        arrow.gameObject.SetActive(true);
                        var angle = VectorToAngle(cell.Coordinates - nearestCell.Coordinates);
                        arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    }
                }
#else
                if (arrow) {
                    Object.Destroy(arrow);
                }
#endif

                queue.Enqueue(nearestCell);
            }
        }

        private float VectorToAngle(Int2 vector)
        {
            var angle = 0f;
            if (vector.x < 0)
            {
                angle = -90f;
            } else if (vector.x > 0)
            {
                angle = 90f;
            } else if (vector.y < 0)
            {
                angle = 0f;
            } else if (vector.y > 0)
            {
                angle = 180f;
            }
            angle += -270f;
            return angle;
        }
    }
}