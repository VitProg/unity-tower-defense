namespace td.features.state
{
    public interface IState
    {
        public float MaxLives { get; set; }
        public float Lives { get; set; }
        public uint LevelNumber { get; set; }
        public int Money { get; set; }
        public float NextWaveCountdown { get; set; }
        public int WaveNumber { get; set; }
        public int WaveCount { get; set; }
        public int EnemiesCount { get; set; }
        public bool IsBuildingProcess { get; set; }
    }
}