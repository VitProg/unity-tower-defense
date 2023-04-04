using System;
using System.Linq;
using td.common;
using td.common.level;
using td.events;
using td.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.services
{
    [Serializable]
    public class LevelData
    {
        [SerializeField]
        private readonly Cell[,] cells = new Cell[Constants.Level.MaxWidth, Constants.Level.MaxHeight];
        
        [SerializeField]
        private readonly Spawn[] spawns = new Spawn[Constants.Level.MaxSpawns];

        [SerializeField]
        private readonly Kernel[] kernels = new Kernel[Constants.Level.MaxTargets];

        [SerializeField]
        public uint SpawnsLength { get; protected set; } = 0;
        [SerializeField]
        public uint KernelsLength { get; protected set; } = 0;

        [SerializeField]
        private LevelConfig? _levelConfig;

        public LevelConfig? LevelConfig
        {
            get => _levelConfig;
            set
            {
                _levelConfig = value;
                _lives = MaxLives;
                money = LevelConfig?.startedMoney ?? 10;
            }
        }

        public int LevelNumber => LevelConfig?.levelNumber ?? -1;

        [SerializeField]
        public int waveNumber = -1;

        public bool IsLastWave => waveNumber >= WavesCount - 1;

        public bool allEnemiesSpawned = false;

        public int WavesCount => LevelConfig?.waves.Length ?? 0;

        public int Width { get; private set; } = -1;
        public int Height { get; private set; } = -1;


        public Spawn[] Spawns => spawns.Where(spawn => spawn != null).ToArray();
        public Kernel[] Kernels => kernels.Where(target => target != null).ToArray();
        
        //todo подумать куда лучше вынестти стейт
        public float MaxLives => LevelConfig?.lives ?? 0;

        private float _lives = 0;
        public float Lives
        {
            get => _lives;
            set {
                if (_lives != value)
                {
                    _lives = value;
                }
            }
        
        }

        public int money = 0;

        public LevelData()
        {
            _lives = MaxLives;
        }

        public void AddCell(Cell cell)
        {
            if (cell.Coordinates.x < 0 || cell.Coordinates.x >= Constants.Level.MaxWidth ||
                cell.Coordinates.y < 0 || cell.Coordinates.y >= Constants.Level.MaxHeight)
            {
                throw new IndexOutOfRangeException();
            }

            cells[cell.Coordinates.x, cell.Coordinates.y] = cell;

            if (Width < cell.Coordinates.x + 1)
            {
                Width = cell.Coordinates.x + 1;
            }

            if (Height < cell.Coordinates.y + 1)
            {
                Height = cell.Coordinates.y + 1;
            }
        }

        public Cell GetCell(Vector2 vector)
        {
            var gridCoordinate = GridUtils.GetGridCoordinate(vector);
            return GetCell(gridCoordinate);
        }
        
        public Cell GetCell(Int2 position)
        {
            return GetCell(position.x, position.y);
        }

        public Cell GetCell(int x, int y)
        {
            if (x is < 0 or > (int)Constants.Level.MaxWidth ||
                y is < 0 or > (int)Constants.Level.MaxHeight)
            {
                return null;
            }
            return cells[x, y];
        }

        public void AddSpawn(Spawn spawn)
        {
            spawns[SpawnsLength] = spawn;
            SpawnsLength++;
        }
        
        public void AddKernel(Kernel kernel)
        {
            kernels[KernelsLength] = kernel;
            KernelsLength++;
        }
        
        
    }
}