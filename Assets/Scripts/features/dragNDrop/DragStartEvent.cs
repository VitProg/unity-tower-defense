namespace td.features.dragNDrop
{
    public struct DragStartEvent
    {
        public DragMode mode;
    }

    public enum DragMode
    {
        World,
        Camera,
    }
}