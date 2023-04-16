using td.common.cells.interfaces;
using UnityEngine;

namespace td.common.cells.hex
{
    public class HexCellCanWalk : HexCell, ICellCanWalk
    {
        public Int2 NextCellCoordinates { get; set; }
        public bool HasNext { get; set; }
        public int DistanceToKernel { get; set; }
        
        public uint Spawn { get; set; }
        public uint Kernel { get; set; }

        public GameObject GameObject { get; set; }

        public bool IsKernel => Kernel > 0;
        public bool IsSpawn => Spawn > 0;
        
        
    }
}