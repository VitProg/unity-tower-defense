using System;
using td.common;
using UnityEngine;

namespace td.components.behaviors
{
    [Serializable]
    [GenerateProvider]
    public struct LinearMovementToTarget
    {
        public Vector2 from;
        public Vector2 target;
        public float gap;
        public float speed;
    }
}