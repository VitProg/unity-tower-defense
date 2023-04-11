using System;

namespace td.components.flags
{
    [Serializable]
    public struct IsDragging
    {
        public double startedTime;
        public IsDraggingMode mode;
    }

    public enum IsDraggingMode
    {
        None,
        Up,
        Down,
    }
}