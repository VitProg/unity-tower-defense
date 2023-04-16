using System.Collections.Generic;
using System.Linq;
using td.common;
using td.common.cells;
using td.common.cells.hex;
using td.common.cells.interfaces;
using td.utils;
using TMPro;
using UnityEngine;

namespace td.services
{
    public class PathService
    {
        private readonly Int2 toLeft = new(-1, 0);
        private readonly Int2 toRight = new(1, 0);
        private readonly Int2 toTop = new(0, 1);
        private readonly Int2 toBottom = new(0, -1);

        private readonly Queue<ICellCanWalk> queue = new();

        private readonly LevelMap levelMap;
        
        public PathService(LevelMap levelMap)
        {
            this.levelMap = levelMap;
        }
        
        public void InitPath()
        {
            queue.Clear();

            Debug.Assert(levelMap.Kernels != null);
            var kernels = levelMap.Kernels;

            uint kernelIndex = 1;
            foreach (var kernel in kernels)
            {
                var kernelCell = levelMap.GetCell<ICellCanWalk>(kernel.Coordinates);
                Debug.Assert(kernelCell != null);
                kernelCell.Kernel = kernelIndex;
                kernelCell.DistanceToKernel = 0;
                queue.Enqueue(kernelCell);
                kernelIndex++;
            }

            do
            {
                var cell = queue.Dequeue();
                Tick(cell);
            } while (queue.Count > 0);

            queue.Clear();
        }

        private void Tick(ICellCanWalk cell)
        {
            var even = cell.Coordinates.x % 2 == 0;

            Int2[] neighborsCoords;
            
            ICellCanWalk[] nearestCells;

            if (cell is HexCellCanWalk hexCell)
            {
                neighborsCoords = new[]
                {
                    hexCell.GetNortWestNeighbor(),
                    hexCell.GetNorthNeighbor(),
                    hexCell.GetNortEastNeighbor(),
                    hexCell.GetSouthEastNeighbor(),
                    hexCell.GetSouthNeighbor(),
                    hexCell.GetSouthWestNeighbor(),
                };
            }
            else
            {
                neighborsCoords= new[]
                {
                    cell.Coordinates - (even ? toLeft : toTop),
                    cell.Coordinates - (even ? toBottom : toLeft),
                    cell.Coordinates - (even ? toRight : toBottom),
                    cell.Coordinates - (even ? toTop : toRight),
                };
            }

            var neighbors = neighborsCoords.Select(coord => levelMap.GetCell<ICellCanWalk>(coord)).ToArray();
            
            foreach (var neighborCell in neighbors)
            {
                if (neighborCell == null || neighborCell.HasNext) continue;

                neighborCell.NextCellCoordinates = cell.Coordinates;
                neighborCell.HasNext = true;
                neighborCell.DistanceToKernel = cell.DistanceToKernel + 1;

                queue.Enqueue(neighborCell);
            }
        }
    }
}