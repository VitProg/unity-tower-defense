// using td.common.cells.hex;
// using td.common.cells.interfaces;
// using td.common.cells.square;
// using td.common.level;
// using td.monoBehaviours;
// using td.utils;
// using UnityEngine;
//
// namespace td.services
// {
//     public static class CellFactory
//     {
//         public static ICellCanWalk MakeCanWalkCell(GameObject cellGameObject, LevelCellType cellType, float cellSize)
//         {
//             var isSwitcher = cellGameObject.transform.GetComponent<Cell>() != null;
//
//             var coordinates = GridUtils.CoordsToCell(
//                 cellGameObject.transform.position,
//                 cellType,
//                 cellSize
//             );
//
//
//             if (cellType == LevelCellType.Hex)
//             {
//                 if (isSwitcher)
//                 {
//                     return new HexCellSwitcherCanWalk()
//                     {
//                         Coords = coordinates,
//                         GameObject = cellGameObject,
//                     };
//                 }
//
//                 return new HexCellCanWalk()
//                 {
//                     Coords = coordinates,
//                     GameObject = cellGameObject,
//                 };
//             }
//
//             if (isSwitcher)
//             {
//                 return new SquareCellSwitcherCanWalk()
//                 {
//                     Coords = coordinates,
//                     GameObject = cellGameObject,
//                 };
//             }
//
//             return new SquareCellCanWalk()
//             {
//                 Coords = coordinates,
//                 GameObject = cellGameObject,
//             };
//         }
//
//         public static ICell MakeCanBuildCell(GameObject cellGameObject, LevelCellType cellType, float cellSize)
//         {
//             var coordinates = GridUtils.CoordsToCell(
//                 cellGameObject.transform.position,
//                 cellType,
//                 cellSize
//             );
//             
//             if (cellType == LevelCellType.Hex)
//             {
//                 return new HexCellCanBuild()
//                 {
//                     Coords = coordinates,
//                     GameObject = cellGameObject
//                 };
//             }
//             else
//             {
//                 return new SquareCellCanBuild()
//                 {
//                     Coords = coordinates,
//                     GameObject = cellGameObject
//                 };
//             }
//         }
//     }
// }