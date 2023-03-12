using System;
using System.Linq;
using td.common;
using td.common.level;
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
        private readonly Target[] targets = new Target[Constants.Level.MaxTargets];

        [SerializeField]
        public uint SpawnsLength { get; protected set; } = 0;
        [SerializeField]
        public uint TargetsLength { get; protected set; } = 0;

        [SerializeField]
        public LevelConfig levelConfig;

        public int LevelNumber => levelConfig.levelNumber;

        [SerializeField]
        public uint waveNumber;

        public bool allEnemiesSpawned = false;

        public uint WavesCount => (uint)levelConfig.waves.Length;

        public int Width { get; private set; } = -1;
        public int Height { get; private set; } = -1;


        public Spawn[] Spawns => spawns.Where(spawn => spawn != null).ToArray();
        public Target[] Targets => targets.Where(target => target != null).ToArray();

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
        
        public void AddTarget(Target target)
        {
            targets[TargetsLength] = target;
            TargetsLength++;
        }
        
    }
}