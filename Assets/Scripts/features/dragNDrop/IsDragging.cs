using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.dragNDrop
{
    [Serializable]
    public struct IsDragging
    {
        public DragMode mode;
        public Vector2 startedPosition;
        public double startedTime;
        [FormerlySerializedAs("mode")] public IsDraggingState state;
        public bool isGridSnapping;
        public string startedLayer;
    }

    public enum IsDraggingState
    {
        None,
        Up,
        Down,
    }
}