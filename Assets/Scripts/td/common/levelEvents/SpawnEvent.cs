
namespace td.common.levelEvents
{
    public struct SpawnEvent : ILevelEvent
    {
        public uint DelayBefore { get; set; }
        public uint Spawner;
        public int[] EnemyTypes;
        public uint DelayBetweenSpawn;
        public uint NumberOfEnemies;
        public float EnemyForceMultiplier;
        public SelectEnemyTypeMethod SelectEnemyTypeMethod;
        public bool WaitEnd;
    }

    public enum SelectEnemyTypeMethod
    {
        Random,
        Sequence,
    }
        
}