using System;
using td.features.dragNDrop.events;

namespace td.features.dragNDrop.flags
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