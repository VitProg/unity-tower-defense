using System;
using UnityEngine;

namespace td.common
{
    [Serializable]
    public class Cell
    {
        public Int2 Coordinates;
        public bool walkable;
        public bool space;
        public int buildEntity;
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