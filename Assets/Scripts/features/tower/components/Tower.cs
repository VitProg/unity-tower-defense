using System;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.tower.components
{
    [Serializable]
    public struct Tower
    {
        public float radius;
        // public uint cost;
        public Vector2 barrel;
        public int2 coords;
    }
}