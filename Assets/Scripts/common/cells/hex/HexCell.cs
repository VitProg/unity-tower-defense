// using System;
// using td.common.cells;
// using td.common.cells.interfaces;
// using td.monoBehaviours;
// using td.utils;
// using UnityEngine;
//
// namespace td.common.cells.hex
// {
//     public abstract class HexCell : ICell
//     {
//         public Int2 Coords { get; set; }
//
//         private int x => Coords.x;
//         private int y => Coords.y;
//
//         private bool IsOddRow => Math.Abs(x) % 2 == 1;
//         
//         // public GameObject GameObject { get; set; }
//         public CellData CellData { get; set; }
//
//         /*
//          * North
//          * West
//          * South
//          * East
//          */
//
//         // public Int2 GetNorthWestNeighbor() =>  IsOddRow ? new(x - 1, y + 1) : new(x - 1, y + 0);
//         // public Int2 GetNorthNeighbor() =>                new(x + 0, y + 1);
//         // public Int2 GetNorthEastNeighbor() =>  IsOddRow ? new(x + 1, y + 1) : new(x + 1, y + 0);
//         //
//         // public Int2 GetSouthEastNeighbor() => IsOddRow ? new(x + 1, y - 0) : new(x + 1, y - 1);
//         // public Int2 GetSouthNeighbor() =>                new(x + 0, y - 1);
//         // public Int2 GetSouthWestNeighbor() => IsOddRow ? new(x - 1, y - 0) : new(x - 1, y - 1);
//         // public Int2 GetNorthWestNeighbor() => HexGridUtils.GetNeighborsCoords(this, HexDirections.NorthWest);
//         // public Int2 GetNorthNeighbor() => HexGridUtils.GetNeighborsCoords(this, HexDirections.NorthWest);
//         // public Int2 GetNorthEastNeighbor() => HexGridUtils.GetNeighborsCoords(this, HexDirections.NorthWest);
//         //
//         // public Int2 GetSouthEastNeighbor() => HexGridUtils.GetNeighborsCoords(this, HexDirections.NorthWest);
//         // public Int2 GetSouthNeighbor() => HexGridUtils.GetNeighborsCoords(this, HexDirections.NorthWest);
//         // public Int2 GetSouthWestNeighbor() => HexGridUtils.GetNeighborsCoords(this, HexDirections.NorthWest);
//
//         public override string ToString()
//         {
//             return $"C: {Coords}";
//         }
//     }
// }