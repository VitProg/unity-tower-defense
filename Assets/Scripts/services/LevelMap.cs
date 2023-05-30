using System.Collections.Generic;
using td.common;
using td.common.level;
using td.features.state;
using td.monoBehaviours;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.services
{
    public class LevelMap
    {
        private uint spawnsLength = 0;
        private uint kernelsLength = 0;

        private Cell[,] cells = new Cell[Constants.Level.MaxMapArrayWidth, Constants.Level.MaxMapArrayHeight];
        private Int2?[] spawns = new Int2?[Constants.Level.MaxSpawns];
        private Int2?[] kernels = new Int2?[Constants.Level.MaxTargets];

        private LevelConfig? levelConfig;

        private int minX = 999;
        private int minY = 999;
        private int maxX = -999;
        private int maxY = -999;
        private readonly List<Cell> prebuildedCells = new();
        
        public LevelConfig? LevelConfig
        {
            get => levelConfig;
            set
            {
                //todo
                levelConfig = value;
                var state = DI.GetCustom<State>();
                if (state == null) return;
                state.SuspendEvents();
                state.MaxLives = levelConfig?.lives ?? 0;
                state.Lives = state.MaxLives;
                state.LevelNumber = levelConfig?.levelNumber ?? 0;
                state.Money = levelConfig?.energy ?? 10;
                state.NextWaveCountdown = 0;
                state.WaveNumber = 0;
                state.WaveCount = levelConfig?.waves.Length ?? 0;
                state.ResumeEvents();
                state.Refresh();
            }
        }

        public int Width { get; private set; } = -1;
        public int Height { get; private set; } = -1;

        private Int2 mapOffset = new(0, 0);

        public Int2[] Spawns => ArrayUtils.NotNullable(spawns);
        public int[] SpawnsIndexse => ArrayUtils.GetIndexes(spawns, ArrayUtils.IsNotNull);
        public Int2[] Kernels => ArrayUtils.NotNullable(kernels);
        public int[] KernelsIndexse => ArrayUtils.GetIndexes(kernels, ArrayUtils.IsNotNull);

        public Rect Rect { get; private set; }

        public LevelMap()
        {
        }

        public void Clear()
        {
            levelConfig = null;
            mapOffset = new Int2(0, 0);
            cells = new Cell[Constants.Level.MaxMapArrayWidth, Constants.Level.MaxMapArrayHeight];
            spawns = new Int2?[Constants.Level.MaxSpawns];
            kernels = new Int2?[Constants.Level.MaxTargets];
            spawnsLength = 0;
            kernelsLength = 0;
            Width = -1;
            Height = -1;
            prebuildedCells.Clear();
            minX = 999;
            minY = 999;
            maxX = -999;
            maxY = -999;
            Rect = new Rect(0, 0, 0, 0);
        }
        public void PreAddCell(Cell cell)
        {
            //todo
            prebuildedCells.Add(cell);

            cell.isPathAnalyzed = false;

            if (minX > cell.Coords.x) minX = cell.Coords.x;
            if (minY > cell.Coords.y) minY = cell.Coords.y;
            if (maxX < cell.Coords.x) maxX = cell.Coords.x;
            if (maxY < cell.Coords.y) maxY = cell.Coords.y;

            if (minX <= maxX && minY <= maxY)
            {
                var c1 = HexGridUtils.CellToPosition(new Int2 { x = minX, y = minY });
                var c2 = HexGridUtils.CellToPosition(new Int2 { x = maxX, y = maxY });
                Rect = new Rect(
                    c1.x, c1.y,
                    c2.x - c1.x, c2.y - c1.y
                );
                // Debug.Log($"> Rect: {Rect} [{c1}; {c2}] [{Width}x{Height}] [{minX}-{maxX};{minY}-{maxY}]");
            }
        }

        public void BuildMap()
        {
            mapOffset = new Int2(minX, minY);
            Width = maxX - minX;
            Height = maxY - minY;

            foreach (var cell in prebuildedCells)
            {
                var oX = cell.Coords.x - mapOffset.x;
                var oY = cell.Coords.y - mapOffset.y;

                cells[oX, oY] = cell;

                if (cell.isKernel)
                {
                    kernels[cell.kernelNumber] = cell.Coords;
                    kernelsLength = cell.kernelNumber > kernelsLength ? cell.kernelNumber : kernelsLength;
                }

                if (cell.isSpawn)
                {
                    spawns[cell.spawnNumber] = cell.Coords;
                    spawnsLength = cell.spawnNumber > spawnsLength ? cell.spawnNumber : spawnsLength;
                }
            }

            prebuildedCells.Clear();
        }
        
        public Int2 GetSpawn(int spawnNumer)
        {
            if (spawnNumer < 0 || spawnNumer >= Constants.Level.MaxSpawns || spawns[spawnNumer] == null)
            {
                return RandomUtils.RandomArrayItem(Spawns);
            }

            return Spawns[spawnNumer];
        }
        
        public bool HasSpawn(int spawnNumer)
        {
            if (spawnNumer < 0 || spawnNumer >= Constants.Level.MaxSpawns || spawns[spawnNumer] == null)
            {
                return false;
            }

            return true;
        }

        public Cell? GetCell(int x, int y, CellTypes? type = null)
        {
            var oX = x - mapOffset.x;
            var oY = y - mapOffset.y;

            if (oX is < 0 or > (int)Constants.Level.MaxMapArrayWidth ||
                oY is < 0 or > (int)Constants.Level.MaxMapArrayHeight)
            {
                return null;
            }
            
            var cell = cells[oX, oY];

            if (type == null || cell == null || cell.type == type) return cell;
            return null;
        }
        public Cell? GetCell(Int2? coords, CellTypes? type = null) =>
            coords != null ? GetCell(coords.Value.x, coords.Value.y, type) : null;
        
        public Cell? GetCell(Vector2 position, CellTypes? type = null) =>
            GetCell(HexGridUtils.PositionToCell(position), type);

        public bool TryGetCell(int x, int y, out Cell cell, CellTypes? type = null)
        {
            cell = GetCell(x, y, type);
            return cell != null;
        }

        public bool TryGetCell(Int2? coords, out Cell cell, CellTypes? type = null)
        {
            if (coords != null) return TryGetCell(coords.Value.x, coords.Value.y, out cell, type);
            cell = null;
            return false;
        }

        public bool TryGetCell(Vector2 position, out Cell cell, CellTypes? type = null) =>
            TryGetCell(HexGridUtils.PositionToCell(position), out cell, type);
        
        public void DebugLogHexMap()
        {
            var xFrom = mapOffset.x;
            var xTo = Width + mapOffset.x;

            var yFrom = Height + mapOffset.y;
            var yTo = mapOffset.y;

            var result = "";

            for (var y = yFrom; y >= yTo; y--)
            {
                var up = "";
                var down = "";

                for (var x = xFrom; x <= xTo; x++)
                {
                    var cell = GetCell(x, y);
                    if (x % 2 == 0)
                    {
                        up += x == xFrom ? "__" : "_";
                        down += "<" + FormatCell(cell) + '>';
                    }
                    else
                    {
                        up += "<" + FormatCell(cell) + '>';
                        down += x == xFrom ? "__" : "_";
                    }
                }

                result += up + "\n" + down + "\n";
            }

            Debug.Log(result);
        }

        private static char FormatCell(Cell cell)
        {
            if (cell == null)
            {
                return '□';
            }

            if (cell.type == CellTypes.CanWalk)
            {
                if (cell.isKernel)
                {
                    return 'K';
                }

                if (cell.isSpawn)
                {
                    return 'S';
                }

                if (cell.isSwitcher)
                {
                    return '⟏';
                }
                
                return '■';
            }

            if (cell.type == CellTypes.CanBuild)
            {
                return '+';
            }

            return ' ';
        }
    }
}