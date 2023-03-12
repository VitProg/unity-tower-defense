namespace td.components.commands
{
    public struct TimeoutBeforeEventCommand<T> where T : struct
    {
        public int Timeout;
        public int Time;
        public T Event;
    }
}