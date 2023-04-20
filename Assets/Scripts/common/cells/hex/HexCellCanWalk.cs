// using td.common.cells.interfaces;
// using UnityEngine;
//
// namespace td.common.cells.hex
// {
//     public class HexCellCanWalk : HexCell, ICellCanWalk
//     {
//         public Int2 NextCellCoordinates { get; set; }
//         public bool HasDirectionToNext { get; set; }
//         public int DistanceToKernel { get; set; }
//         public int distanceFromSpawn { get; set; }
//
//         public uint Spawn { get; set; }
//         public uint Kernel { get; set; }
//
//         public GameObject GameObject { get; set; }
//
//         public bool IsKernel => Kernel > 0;
//         public bool IsSpawn => Spawn > 0;
//         
//         public override string ToString()
//         {
//             return $"CW: {Coords}, nCoords{NextCellCoordinates}, dk{DistanceToKernel}, s{Spawn}, k{Kernel}";
//         }
//     }
// }