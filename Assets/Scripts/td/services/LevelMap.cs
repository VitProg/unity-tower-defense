using System;
using System.Linq;
using JetBrains.Annotations;
using td.common;
using td.common.cells;
using td.common.level;
using td.monoBehaviours;
using td.states;
using td.utils;
using UnityEngine;

namespace td.services
{
    public class LevelMap
    {
        private uint spawnsLength = 0;
        private uint kernelsLength = 0;

        private ICell[,] cells = new ICell[Constants.Level.MaxMapArrayWidth, Constants.Level.MaxMapArrayHeight];
        private Spawn[] spawns = new Spawn[Constants.Level.MaxSpawns];
        private Kernel[] kernels = new Kernel[Constants.Level.MaxTargets];

        private LevelConfig? levelConfig;
        private readonly LevelState levelState;

        public LevelConfig? LevelConfig
        {
            get => levelConfig;
            set
            {
                levelConfig = value;
                levelState.MaxLives = levelConfig?.lives ?? 0;
                levelState.Lives = levelState.MaxLives;
                levelState.LevelNumber = levelConfig?.levelNumber ?? 0;
                levelState.Money = levelConfig?.startedMoney ?? 10;
                levelState.NextWaveCountdown = 0;
                levelState.WaveNumber = 0;
                levelState.WaveCount = levelConfig?.waves.Length ?? 0;
            }
        }

        public int Width { get; private set; } = -1;
        public int Height { get; private set; } = -1;

        private Int2 mapOffset = new(0, 0);

        public Spawn[] Spawns => spawns.Where(spawn => spawn != null).ToArray();
        public Kernel[] Kernels => kernels.Where(target => target != null).ToArray();


        public LevelMap(LevelState levelState)
        {
            this.levelState = levelState;
        }

        public void Clear()
        {
            levelConfig = null;
            mapOffset = new Int2(0, 0);
            cells = new ICell[Constants.Level.MaxMapArrayWidth, Constants.Level.MaxMapArrayHeight];
            spawns = new Spawn[Constants.Level.MaxSpawns];
            kernels = new Kernel[Constants.Level.MaxTargets];
            spawnsLength = 0;
            kernelsLength = 0;
            Width = -1;
            Height = -1;
        }

        public void AddCell(ICell cell)
        {
            if (cell.Coordinates.x < 0 && cell.Coordinates.x < mapOffset.x)
            {
                var length = (cell.Coordinates.x * -1);
                ShiftMapRight(length + mapOffset.x);
            }

            if (cell.Coordinates.y < 0 && cell.Coordinates.y < mapOffset.y)
            {
                var length = (cell.Coordinates.y * -1);
                ShiftMapTop(length + mapOffset.y);
            }

            // if (
            //     cell.Coordinates.x < mapOffset.x ||
            //     cell.Coordinates.y < mapOffset.y ||
            //     cell.Coordinates.x + mapOffset.x >= Constants.Level.MaxMapArrayWidth ||
            //     cell.Coordinates.y + mapOffset.y >= Constants.Level.MaxMapArrayHeight
            // ) {
            //     throw new IndexOutOfRangeException(
            //         $"[{cell.Coordinates.x}; {cell.Coordinates.y}]-[{mapOffset.x}; {mapOffset.y}] - out of map range [{Constants.Level.MaxMapArrayWidth}; {Constants.Level.MaxMapArrayHeight}]"
            //     );
            // }

            var oX = cell.Coordinates.x - mapOffset.x;
            var oY = cell.Coordinates.y - mapOffset.y;

            try
            {
                cells[oX, oY] = cell;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (Width < oX + 1)
            {
                Width = oX + 2;
            }

            if (Height < oY + 1)
            {
                Height = oY + 2;
            }

            if (Width >= Constants.Level.MaxMapArrayWidth || Height >= Constants.Level.MaxMapArrayHeight)
            {
                throw new IndexOutOfRangeException(
                    $"Map [{Width}; {Height}] - out of map range [{Constants.Level.MaxMapArrayWidth}; {Constants.Level.MaxMapArrayHeight}"
                );
            }

#if UNITY_EDITOR
            if (cell.GetType() == typeof(CellCanWalk))
            {
                var cellInfo = ((CellCanWalk)cell).gameObject.GetComponent<CellInfo>();
                if (cellInfo != null)
                {
                    cellInfo.SetCell(cell);
                }
            }
#endif
        }
        
        [CanBeNull]
        public ICell GetCell(int x, int y)
        {
            var oX = x - mapOffset.x;
            var oY = y - mapOffset.y;

            if (oX is < 0 or > (int)Constants.Level.MaxMapArrayWidth ||
                oY is < 0 or > (int)Constants.Level.MaxMapArrayHeight)
            {
                return null;
            }

            return cells[oX, oY];
        }

        [CanBeNull]
        public T GetCell<T>(Vector2 vector) where T : class, ICell
        {
            var gridCoordinate = GridUtils.GetGridCoordinate(vector);
            return GetCell<T>(gridCoordinate);
        }
        
        public bool TryGetCell<T>(Vector2 vector, out T cell) where T : class, ICell
        {
            cell = GetCell<T>(vector);
            return cell != null;
        }


        [CanBeNull]
        public T GetCell<T>(Int2 position) where T : class, ICell
        {
            return GetCell<T>(position.x, position.y);
        }
        
        public bool TryGetCell<T>(Int2 position, out T cell) where T : class, ICell
        {
            cell = GetCell<T>(position);
            return cell != null;
        }

        
        [CanBeNull]
        public T GetCell<T>(int x, int y) where T : class, ICell
        {
            var cell = GetCell(x, y);
            return cell != null && cell.GetType() == typeof(T) ? (T)cell : null;
        }
        
        public bool TryGetCell<T>(int x, int y, out T cell) where T : class, ICell
        {
            cell = GetCell<T>(x, y);
            return cell != null;
        }


        public void AddSpawn(Spawn spawn)
        {
            spawns[spawnsLength] = spawn;
            spawnsLength++;
            var cell = GetCell<CellCanWalk>(spawn.Coordinates);
            if (cell != null)
            {
                cell.spawn = spawnsLength;
            }
        }

        public void AddKernel(Kernel kernel)
        {
            kernels[kernelsLength] = kernel;
            kernelsLength++;
            var cell = GetCell<CellCanWalk>(kernel.Coordinates);
            if (cell != null)
            {
                cell.kernel = kernelsLength;
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        private void ShiftMapRight(int length)
        {
            mapOffset = new Int2(mapOffset.x - length, mapOffset.y);

            for (var y = 0; y < (int)Constants.Level.MaxMapArrayHeight; y++)
            {
                for (var x = (int)Constants.Level.MaxMapArrayWidth - length - 1; x >= 0; x--)
                {
                    cells[x + length, y] = cells[x, y];
                    cells[x, y] = null;
                }
            }

#if UNITY_EDITOR
            // Debug.Log($@"After ShiftMapRight {length}");
            // ShowMapInLog();
#endif
        }

        private void ShiftMapTop(int length)
        {
            mapOffset = new Int2(mapOffset.x, mapOffset.y - length);

            for (var y = length; y < (int)Constants.Level.MaxMapArrayHeight; y++)
            {
                for (var x = (int)Constants.Level.MaxMapArrayWidth - 1; x >= 0; x--)
                {
                    cells[x, y - length] = cells[x, y];
                    cells[x, y] = null;
                }
            }

#if UNITY_EDITOR
            // Debug.Log($@"After ShiftMapTop {length}");
            // ShowMapInLog();
#endif
        }

        public void ShowMapInLog()
        {
            var line = "GetCell:\n";

            var xFrom = mapOffset.x;
            var xTo = Width + mapOffset.x - 1;

            var yFrom = Height + mapOffset.y - 1;
            var yTo = mapOffset.y;

            line += $"X: {xFrom}...{xTo}\n";
            line += $"Y: {yFrom}...{yTo}\n";

            for (var y = yFrom; y >= yTo; y--)
            {
                line += Math.Abs(y).ToString("D2") + ": ";
                for (var x = xFrom; x <= xTo; x++)
                {
                    line += FormatCell(GetCell<CellCanWalk>(x, y));
                }

                line += '\n';
            }

            //////////////////////////////////////////

            line += "\n\n---------------------------------------\n\nFrom array:\n";
            line += $"X: {0}...{Width - 1}\n";
            line += $"Y: {Height - 1}...{0}\n";
            for (var y = Height - 1; y >= 0; y--)
            {
                line += Math.Abs(y).ToString("D2") + ": ";
                for (var x = 0; x < Width; x++)
                {
                    var cell = cells[x, y];
                    if (cell != null && cell.GetType() == typeof(CellCanWalk))
                    {
                        line += FormatCell((CellCanWalk)cell);
                    }
                }

                line += '\n';
            }

            Debug.Log(line);
        }

        private static char FormatCell(CellCanWalk cell)
        {
            if (cell == null)
            {
                return '□';
            }

            if (cell.kernel > 0)
            {
                return 'K';
            }

            if (cell.spawn > 0)
            {
                return 'S';
            }

            return '■';
        }
    }

    public enum CellType
    {
        CanWalk,
        CanBuild,
        WithBuilding,
    }
}