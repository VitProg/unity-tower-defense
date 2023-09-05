// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using td.features.level.cells;
// using td.features.level.data;
// using td.utils;
// using Unity.Mathematics;
// using UnityEngine;
//
// namespace td.features.level
// {
//     public class LevelMap
//     {
//         public byte SpawnCount { get; private set; }
//         public byte KernelCount { get; private set; }
//
//         private readonly Cell[,] cells = new Cell[Constants.Level.MaxMapArrayWidth, Constants.Level.MaxMapArrayHeight];
//         public readonly int2?[] spawns = new int2?[Constants.Level.MaxSpawns];
//         public readonly int2?[] kernels = new int2?[Constants.Level.MaxKernels];
//
//         private LevelConfig? levelConfig;
//
//         private int minX = 999;
//         private int minY = 999;
//         private int maxX = -999;
//         private int maxY = -999;
//         private readonly List<Cell> prebuildedCells = new();
//
//         public LevelConfig? LevelConfig
//         {
//             get => levelConfig;
//             set
//             {
//                 //todo
//                 levelConfig = value;
//             }
//         }
//
//         public int Width { get; private set; } = -1;
//         public int Height { get; private set; } = -1;
//
//         private int2 mapOffset = new(0, 0);
//         
//         public Rect Rect { get; private set; }
//
//         public void Clear()
//         {
//             levelConfig = null;
//             mapOffset = new int2(0, 0);
//             
//             for (var y = 0; y < Constants.Level.MaxMapArrayHeight; y++)
//                 for (var x = 0; x < Constants.Level.MaxMapArrayWidth; x++)
//                     cells[x, y] = default;
//             
//             for (var i = 0; i < Constants.Level.MaxSpawns; i++)
//                 spawns[i] = null;
//             
//             for (var i = 0; i < Constants.Level.MaxKernels; i++)
//                 kernels[i] = null;
//             
//             SpawnCount = 0;
//             KernelCount = 0;
//             Width = -1;
//             Height = -1;
//             prebuildedCells.Clear();
//             minX = 999;
//             minY = 999;
//             maxX = -999;
//             maxY = -999;
//             Rect = new Rect(0, 0, 0, 0);
//         }
//         public void PreAddCell(Cell cell)
//         {
//             //todo
//             prebuildedCells.Add(cell);
//
//             cell.isPathAnalyzed = false;
//
//             if (minX > cell.coords.x) minX = cell.coords.x;
//             if (minY > cell.coords.y) minY = cell.coords.y;
//             if (maxX < cell.coords.x) maxX = cell.coords.x;
//             if (maxY < cell.coords.y) maxY = cell.coords.y;
//
//             if (minX <= maxX && minY <= maxY)
//             {
//                 var c1 = HexGridUtils.CellToPosition(new int2(minX, minY));
//                 var c2 = HexGridUtils.CellToPosition(new int2(maxX, maxY));
//                 Rect = new Rect(
//                     c1.x, c1.y,
//                     c2.x - c1.x, c2.y - c1.y
//                 );
//             }
//         }
//
//         public void BuildMap()
//         {
//             mapOffset = new int2(minX, minY);
//             Width = maxX - minX;
//             Height = maxY - minY;
//
//             foreach (var cell in prebuildedCells)
//             {
//                 var oX = cell.coords.x - mapOffset.x;
//                 var oY = cell.coords.y - mapOffset.y;
//
//                 cells[oX, oY] = cell;
//
//                 if (cell.isKernel)
//                 {
//                     kernels[cell.kernelNumber] = cell.coords;
//                     KernelCount++;// = cell.kernelNumber > kernelsLength ? cell.kernelNumber : kernelsLength;
//                 }
//
//                 if (cell.isSpawn)
//                 {
//                     spawns[cell.spawnNumber] = cell.coords;
//                     SpawnCount++;// = cell.spawnNumber > spawnsLength ? cell.spawnNumber : spawnsLength;
//                 }
//             }
//
//             if (spawns[SpawnCount - 1] == null) 
//                 Debug.LogError($"Spawn numbers are out of order in level {(levelConfig.HasValue ? levelConfig.Value.levelNumber : "?")}");
//             
//             if (kernels[KernelCount - 1] == null) 
//                 Debug.LogError($"Kernel numbers are out of order in level {(levelConfig.HasValue ? levelConfig.Value.levelNumber : "?")}");
//             
//             
//             
//             //todo проверить идут ли номера спакнов и ядер подряд и нет ли пропусков!!!
//
//             prebuildedCells.Clear();
//         }
//         
//         public int2 GetSpawn(int spawmNumber)
//         {
//             if (spawmNumber >= 0 && spawmNumber < Constants.Level.MaxSpawns && spawns[spawmNumber] != null)
//             {
//                 return spawns[spawmNumber].Value;
//             }
//
//             var randomIndex = RandomUtils.IntRange(0, SpawnCount - 1);
//             // ReSharper disable once PossibleInvalidOperationException
//             return spawns[randomIndex].Value;
//         }
//         
//         public bool HasSpawn(int spawnNumer)
//         {
//             if (spawnNumer < 0 || spawnNumer >= SpawnCount || !spawns[spawnNumer].HasValue)
//             {
//                 return false;
//             }
//
//             return true;
//         }
//
//         
//         [MethodImpl (MethodImplOptions.AggressiveInlining)]
//         public ref Cell GetCell(int2 coords, CellTypes? type = null) => ref GetCell(coords.x, coords.y, type);
//         [MethodImpl (MethodImplOptions.AggressiveInlining)]
//         public ref Cell GetCell(Vector2 position, CellTypes? type = null) => ref GetCell(HexGridUtils.PositionToCell(position), type);
//         [MethodImpl (MethodImplOptions.AggressiveInlining)]
//         public ref Cell GetCell(float x, float y, CellTypes? type = null) => ref GetCell(HexGridUtils.PositionToCell(x, y), type);
//         [MethodImpl (MethodImplOptions.AggressiveInlining)]
//         public ref Cell GetCell(int x, int y, CellTypes? type = null)
//         {
//             var oX = x - mapOffset.x;
//             var oY = y - mapOffset.y;
//
//             return ref cells[oX, oY];
//         }
//
//         
//         [MethodImpl (MethodImplOptions.AggressiveInlining)]
//         public bool HasCell(int2? coords, CellTypes? type = null) => coords != null && HasCell(coords.Value.x, coords.Value.y, type);
//         [MethodImpl (MethodImplOptions.AggressiveInlining)]
//         public bool HasCell(Vector2 position, CellTypes? type = null) => HasCell(HexGridUtils.PositionToCell(position), type);
//         public bool HasCell(int x, int y, CellTypes? type = null)
//         {
//             var oX = x - mapOffset.x;
//             var oY = y - mapOffset.y;
//
//             if (oX is < 0 or > (int)Constants.Level.MaxMapArrayWidth ||
//                 oY is < 0 or > (int)Constants.Level.MaxMapArrayHeight)
//             {
//                 return false;
//             }
//
//             ref var cell = ref cells[oX, oY];
//
//             if (cell.IsEmpty) return false;
//             return type == null || cell.type == type;
//         }
//
//         // public Cell? GetCell(int2? coords, CellTypes? type = null) =>
//             // coords != null ? GetCell(coords.Value.x, coords.Value.y, type) : null;
//         
//         // [CanBeNull]
//         // public Cell? GetCell(Vector2 position, CellTypes? type = null) =>
//             // GetCell(HexGridUtils.PositionToCell(position), type);
//
//         // public bool TryGetCell(int x, int y, out Cell? cell, CellTypes? type = null)
//         // {
//             // cell = GetCell(x, y, type);
//             // return cell != null;
//         // }
//
//         // public bool TryGetCell(int2? coords, out Cell? cell, CellTypes? type = null)
//         // {
//             // if (coords != null) return TryGetCell(coords.Value.x, coords.Value.y, out cell, type);
//             // cell = null;
//             // return false;
//         // }
//
//         // public bool TryGetCell(Vector2 position, out Cell? cell, CellTypes? type = null) =>
//             // TryGetCell(HexGridUtils.PositionToCell(position), out cell, type);
// #if DEBUG
//         public void DebugLogHexMap()
//         {
//             var xFrom = mapOffset.x;
//             var xTo = Width + mapOffset.x;
//
//             var yFrom = Height + mapOffset.y;
//             var yTo = mapOffset.y;
//
//             var result = "";
//
//             for (var y = yFrom; y >= yTo; y--)
//             {
//                 var up = "";
//                 var down = "";
//
//                 for (var x = xFrom; x <= xTo; x++)
//                 {
//                     var cell = GetCell(x, y);
//                     if (x % 2 == 0)
//                     {
//                         up += x == xFrom ? "__" : "_";
//                         down += "<" + FormatCell(cell) + '>';
//                     }
//                     else
//                     {
//                         up += "<" + FormatCell(cell) + '>';
//                         down += x == xFrom ? "__" : "_";
//                     }
//                 }
//
//                 result += up + "\n" + down + "\n";
//             }
//
//             Debug.Log(result);
//         }
//
//         private static char FormatCell(Cell? cell)
//         {
//             if (cell == null)
//             {
//                 return '□';
//             }
//
//             if (cell.Value.type == CellTypes.CanWalk)
//             {
//                 if (cell.Value.isKernel)
//                 {
//                     return 'K';
//                 }
//
//                 if (cell.Value.isSpawn)
//                 {
//                     return 'S';
//                 }
//
//                 if (cell.Value.isSwitcher)
//                 {
//                     return '⟏';
//                 }
//                 
//                 return '■';
//             }
//
//             if (cell.Value.type == CellTypes.CanBuild)
//             {
//                 return '+';
//             }
//
//             return ' ';
//         }
// #endif
//     }
// }