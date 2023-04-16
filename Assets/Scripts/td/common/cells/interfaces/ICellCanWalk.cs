using UnityEngine;

namespace td.common.cells.interfaces
{
    public interface ICellCanWalk : ICell
    {
        public Int2 NextCellCoordinates { get; set; }
        public bool HasNext { get; set; }
        public int DistanceToKernel  { get; set; }
        
        public uint Spawn { get; set; }
        public uint Kernel { get; set; }

        public GameObject GameObject { get; set; }

        public bool IsKernel { get; }
        public bool IsSpawn  { get; }
    }
}