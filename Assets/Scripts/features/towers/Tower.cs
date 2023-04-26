using System;
using td.common;
using UnityEngine;

namespace td.features.towers
{
    [Serializable]
    [GenerateProvider]
    public struct Tower
    {
        public float radius;
        public int cost;
        public Vector2 barrel;

        public GameObject radiusGameObject;
    }
}