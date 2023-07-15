using System;
using td.common;
using UnityEngine;
using UnityEngine.Serialization;

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
        public bool resetAnchoredPositionZ;
        public bool speedOfGameAffected;
    }
}