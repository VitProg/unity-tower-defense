namespace td.common.levelEvents
{
    public struct DelayForNextManipulation: ILevelEvent
    {
        public uint DelayBefore { get; set; }
    }
}