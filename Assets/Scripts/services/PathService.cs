// using System.Collections.Generic;
// using System.Linq;
// using td.common;
// using td.common.cells.hex;
// using td.common.cells.interfaces;
// using UnityEngine;
//
// namespace td.services
// {
//     public class PathService : IPathService
//     {
//         private readonly Int2 toLeft = new(-1, 0);
//         private readonly Int2 toRight = new(1, 0);
//         private readonly Int2 toTop = new(0, 1);
//         private readonly Int2 toBottom = new(0, -1);
//
//         private readonly Queue<ICellCanWalk> queue = new();
//
//         private LevelMap levelMap;
//         
//         public void InitPath(LevelMap levelMap)
//         {
//             this.levelMap = levelMap;
//             queue.Clear();
//
//             Debug.Assert(levelMap.Kernels != null);
//             var kernels = levelMap.Kernels;
//
//             uint kernelIndex = 1;
//             foreach (var kernel in kernels)
//             {
//                 var kernelCell = levelMap.GetCell<ICellCanWalk>(kernel.Coordinates);
//                 Debug.Assert(kernelCell != null);
//                 kernelCell.Kernel = kernelIndex;
//                 kernelCell.DistanceToKernel = 0;
//                 queue.Enqueue(kernelCell);
//                 kernelIndex++;
//             }
//
//             if (queue.Count <= 0)
//             {
//                 Debug.LogWarning("Kernels Not Founds!");
//             }
//             else
//             {
//                 do
//                 {
//                     var cell = queue.Dequeue();
//                     Tick(cell);
//                 } while (queue.Count > 0);
//             }
//
//             queue.Clear();
//         }
//
//         private void Tick<T>(T cell) where T : ICellCanWalk
//         {
//             var even = cell.Coords.x % 2 == 0;
//
//             Int2[] `;
//             
//             if (cell is HexCellCanWalk hexCell)
//             {
//                 neighborsCoords = new[]
//                 {
//                     hexCell.GetNorthWestNeighbor(),
//                     hexCell.GetNorthNeighbor(),
//                     hexCell.GetNorthEastNeighbor(),
//                     hexCell.GetSouthEastNeighbor(),
//                     hexCell.GetSouthNeighbor(),
//                     hexCell.GetSouthWestNeighbor(),
//                 };
//             }
//             else
//             {
//                 neighborsCoords= new[]
//                 {
//                     cell.Coords - (even ? toLeft : toTop),
//                     cell.Coords - (even ? toBottom : toLeft),
//                     cell.Coords - (even ? toRight : toBottom),
//                     cell.Coords - (even ? toTop : toRight),
//                 };
//             }
//
//             var neighbors = neighborsCoords.Select(coord => levelMap.GetCell<ICellCanWalk>(coord)).ToArray();
//             
//             foreach (var neighborCell in neighbors)
//             {
//                 if (neighborCell is ICellSwitcherCanWalk switcherCell)
//                 {
//                     if (switcherCell.HasDirectionToNext && switcherCell.HasAlternativeNext) continue;
//
//                     if (!switcherCell.HasDirectionToNext)
//                     {
//                         switcherCell.NextCellCoordinates = cell.Coords;
//                         switcherCell.HasDirectionToNext = true;
//                         switcherCell.DistanceToKernel = cell.DistanceToKernel + 1;
//                     }
//                     else
//                     {
//                         switcherCell.AlternativeNextCellCoordinates = cell.Coords;
//                         switcherCell.HasAlternativeNext = true;
//                         switcherCell.AlternativeDistanceToKernel = cell.DistanceToKernel + 1;
//                     }
//
//                     queue.Enqueue(switcherCell);
//                 }
//                 else
//                 {
//                     if (neighborCell == null || neighborCell.HasDirectionToNext) continue;
//
//                     neighborCell.NextCellCoordinates = cell.Coords;
//                     neighborCell.HasDirectionToNext = true;
//                     neighborCell.DistanceToKernel = cell.DistanceToKernel + 1;
//
//                     queue.Enqueue(neighborCell);
//                 }
//             }
//         }
//     }
// }