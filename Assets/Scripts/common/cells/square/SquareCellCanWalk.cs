// using System;
// using td.common.cells.interfaces;
// using td.monoBehaviours;
// using UnityEngine;
//
// namespace td.common.cells.square
// {
//     [Serializable]
//     public class SquareCellCanWalk : ICell, ICellCanWalk
//     {
//         public Int2 Coords { get; set; }
//
//         public Int2 NextCellCoordinates { get; set; }
//         public bool HasDirectionToNext { get; set; }
//         public int DistanceToKernel { get; set; }
//         public int distanceFromSpawn { get; set; }
//
//         public uint Spawn { get; set; }
//         public uint Kernel { get; set; }
//
//         // public GameObject GameObject { get; set; }
//         public CellData CellData { get; set; }
//
//         public bool IsKernel => Kernel > 0;
//         public bool IsSpawn => Spawn > 0;
//
//         public override string ToString()
//         {
//             var k = IsKernel ? $"| k{Kernel}" : "";
//             var s = IsSpawn ? $"| s{Spawn}" : "";
//             return @$"{Coords} -> {NextCellCoordinates}{k}{s} | dk{DistanceToKernel}";
//         }
//     }
// }