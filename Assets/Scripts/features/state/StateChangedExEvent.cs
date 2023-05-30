namespace td.features.state
{
    public struct StateChangedExEvent
    {
        public int? newShardInCollection;
        public int? removeShardFromCollection;
    }
}