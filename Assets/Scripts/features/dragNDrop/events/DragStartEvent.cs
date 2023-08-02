namespace td.features.dragNDrop.events
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