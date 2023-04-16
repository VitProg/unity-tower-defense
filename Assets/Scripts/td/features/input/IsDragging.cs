using System;

namespace td.features.input
{
    [Serializable]
    public struct IsDragging
    {
        public double startedTime;
        public IsDraggingMode mode;
        public bool isGridSnapping;
    }

    public enum IsDraggingMode
    {
        None,
        Up,
        Down,
    }
}