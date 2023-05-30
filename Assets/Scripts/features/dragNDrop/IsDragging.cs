using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.dragNDrop
{
    [Serializable]
    public struct IsDragging
    {
        public DragMode mode;
        public IsDraggingState state;
        public bool isGridSnapping;
    }

    public enum IsDraggingState
    {
        None,
        Up,
        Down,
    }
}