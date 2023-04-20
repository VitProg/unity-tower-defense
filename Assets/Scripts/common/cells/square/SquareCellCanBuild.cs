// using System;
// using Leopotam.EcsLite;
// using td.common.cells.interfaces;
// using td.monoBehaviours;
// using UnityEngine;
//
// namespace td.common.cells.square
// {
//     [Serializable]
//     public class SquareCellCanBuild: ICell, ICellCanBuild
//     {
//         public Int2 Coords { get; set; }
//         public EcsPackedEntity? BuildingPackedEntity { get; set; }
//         public bool HasBuilding => BuildingPackedEntity != null;
//         public CellData CellData { get; set; }
//     }
// }