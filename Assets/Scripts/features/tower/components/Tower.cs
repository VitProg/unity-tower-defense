using System;
using td.common;
using UnityEngine;

namespace td.features.tower.components
{
    [Serializable]
    public struct Tower
    {
        public float radius;
        // public uint cost;
        public Vector2 barrel;
        public Int2 coords;

        // public GameObject radiusGameObject;
    }
}