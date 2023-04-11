using System;
using UnityEngine;

namespace td.common.cells
{
    [Serializable]
    public class CellCanWalk : ICell
    {
        public Int2 Coordinates { get; set; }
        
        public Int2 NextCellCoordinates;
        public bool hasNext;
        public int distanceToKernel;
        
        public uint spawn;
        public uint kernel;

        public GameObject gameObject;

        public bool IsKernel => kernel > 0;
        public bool IsSpawn => spawn > 0;
    }
}