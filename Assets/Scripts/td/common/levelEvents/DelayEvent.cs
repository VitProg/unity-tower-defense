namespace td.common.levelEvents
{
    public struct DelayEvent: ILevelEvent
    {
        public uint DelayBefore { get; set; }
        public uint Delay { get; set; }
    }
}