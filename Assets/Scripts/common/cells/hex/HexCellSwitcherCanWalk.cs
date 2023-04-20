// using td.common.cells.interfaces;
// using UnityEngine;
//
// namespace td.common.cells.hex
// {
//     public class HexCellSwitcherCanWalk : HexCellCanWalk, ICellSwitcherCanWalk
//     {
//         public Int2 AlternativeNextCellCoordinates { get; set; }
//         public bool HasAlternativeNext { get; set; }
//         public int AlternativeDistanceToKernel { get; set; }
//         
//         public override string ToString()
//         {
//             return $"CSW: {Coords}, nCoords{NextCellCoordinates}, nAltCoords:{AlternativeNextCellCoordinates}, dk{DistanceToKernel}, s{Spawn}, k{Kernel}";
//         }
//     }
// }