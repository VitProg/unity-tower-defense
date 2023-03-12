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
        public bool isTarget; // todo - rename it to "kernel" or "core" or "home"...
        public int buildEntity;
        public Int2 NextCellCoordinates;
        public GameObject gameObject;
    }
}