using td.common;
using td.common.level;
using td.services;
using UnityEngine;

namespace td.utils
{
    public static class GridUtils
    {
        public static Int2 CoordsToCell(Vector2 position, LevelCellType cellType, float cellSize) =>
            cellType == LevelCellType.Hex ? HexGridUtils.CoordsToCell(position, cellSize) : SquareGridUtils.CoordsToCell(position, cellSize);

        public static Vector2 CellToCoords(Int2 coordinate, LevelCellType cellType, float cellSize) =>
            cellType == LevelCellType.Hex ? HexGridUtils.CellToCoords(coordinate, cellSize) : SquareGridUtils.CellToCoords(coordinate, cellSize);

        public static Vector2 SnapToGrid(Vector2 position, LevelCellType cellType, float cellSize) =>
            cellType == LevelCellType.Hex ? HexGridUtils.SnapToGrid(position, cellSize) : SquareGridUtils.SnapToGrid(position, cellSize);
    }
}