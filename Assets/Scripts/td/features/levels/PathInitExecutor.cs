using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.levels
{
    // todo rewrite to load level service
    public class PathInitExecutor : IEcsRunSystem
    {
        private readonly Int2 toLeft = new(-1, 0);
        private readonly Int2 toRight = new(1, 0);
        private readonly Int2 toTop = new(0, 1);
        private readonly Int2 toBottom = new(0, -1);

        private readonly Queue<Cell> queue = new();

        [EcsInject] private LevelMap levelMap;
        
        private readonly EcsFilterInject<Inc<PathInitOuterCommand>> eventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            if (eventEntities.Value.GetEntitiesCount() == 0) return;
            systems.CleanupOuter(eventEntities);

            // Debug.Log("PathInitExecutor RUN...");

            queue.Clear();

            Debug.Assert(levelMap.Kernels != null);
            var kernels = levelMap.Kernels;

            uint kernelIndex = 1;
            foreach (var kernel in kernels)
            {
                var kernelCell = levelMap.GetCell(kernel.Coordinates);
                Debug.Assert(kernelCell != null);
                kernelCell.kernel = kernelIndex;
                kernelCell.distanceToKernel = 0;
                queue.Enqueue(kernelCell);
                kernelIndex++;
            }

            do
            {
                var cell = queue.Dequeue();
                Tick(cell);
            } while (queue.Count > 0);

            systems.SendOuter<LevelLoadedOuterEvent>();

            queue.Clear();

            systems.CleanupOuter(eventEntities);

            // Debug.Log("PathInitExecutor FIN");
        }

        private void Tick(Cell cell)
        {
            var even = cell.Coordinates.x % 2 == 0;

            var nearestCells = new Cell[4]
            {
                levelMap.GetCell(cell.Coordinates - (even ? toLeft : toTop)),
                levelMap.GetCell(cell.Coordinates - (even ? toBottom : toLeft)),
                levelMap.GetCell(cell.Coordinates - (even ? toRight : toBottom)),
                levelMap.GetCell(cell.Coordinates - (even ? toTop : toRight)),
            };

            foreach (var nearestCell in nearestCells)
            {
                if (nearestCell == null || nearestCell.hasNext) continue;

                nearestCell.NextCellCoordinates = cell.Coordinates;
                nearestCell.hasNext = true;
                nearestCell.distanceToKernel = cell.distanceToKernel + 1;

                var arrow = nearestCell.gameObject.transform.Find("arrow");

#if UNITY_EDITOR
                if (arrow && !nearestCell.IsKernel)
                {
                    var vector = cell.Coordinates - nearestCell.Coordinates;
                    if (!vector.empty)
                    {
                        arrow.gameObject.SetActive(true);
                        var angle = VectorToAngle(cell.Coordinates - nearestCell.Coordinates);
                        arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                        if (nearestCell.IsKernel)
                        {
                            arrow.gameObject.SetActive(false);
                        }
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
            }
            else if (vector.x > 0)
            {
                angle = 90f;
            }
            else if (vector.y < 0)
            {
                angle = 0f;
            }
            else if (vector.y > 0)
            {
                angle = 180f;
            }

            angle += -270f;
            return angle;
        }
    }
}