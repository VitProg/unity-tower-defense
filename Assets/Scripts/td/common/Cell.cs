using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.common
{
    [Serializable]
    public class Cell
    {
        public Int2 Coordinates;
        public bool walkable;
        public bool space;
        public bool isKernel;
        public int buildEntity;
        public Int2 NextCellCoordinates;
        public int distanceToKernel;
        
        public GameObject gameObject;
    }
}