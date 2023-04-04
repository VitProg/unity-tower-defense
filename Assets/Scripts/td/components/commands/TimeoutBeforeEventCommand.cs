using System;

namespace td.components.commands
{
    [Serializable]
    public struct TimeoutBeforeEventCommand<T> where T : struct
    {
        public int Timeout;
        public int Time;
        public T Event;
    }
}