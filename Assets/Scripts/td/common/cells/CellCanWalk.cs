using System;
using td.common.cells.interfaces;
using UnityEngine;

namespace td.common.cells
{
    [Serializable]
    public class CellCanWalk : ICell, ICellCanWalk
    {
        public Int2 Coordinates { get; set; }
        
        public Int2 NextCellCoordinates { get; set; }
        public bool HasNext { get; set; }
        public int DistanceToKernel { get; set; }
        
        public uint Spawn { get; set; }
        public uint Kernel { get; set; }

        public GameObject GameObject { get; set; }

        public bool IsKernel => Kernel > 0;
        public bool IsSpawn => Spawn > 0;

        public override string ToString()
        {
            var k = IsKernel ? $"| k{Kernel}" : "";
            var s = IsSpawn ? $"| s{Spawn}" : "";
            return @$"{Coordinates} -> {NextCellCoordinates}{k}{s} | dk{DistanceToKernel}";
        }
    }
}