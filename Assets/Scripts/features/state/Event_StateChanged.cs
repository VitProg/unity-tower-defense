using System.Runtime.CompilerServices;

namespace td.features.state
{
    public struct Event_StateChanged : IStateChangedEvent
    {
        public bool maxLives;
        public bool lives;
        public bool levelNumber;
        public bool energy;
        public bool nextWaveCountdown;
        public bool waveNumber;
        public bool waveCount;
        public bool activeSpawnCount;
        public bool enemiesCount;
        public bool gameSpeed;
        public bool gameSimulationEnabled;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !maxLives && !lives && !levelNumber && !energy && !nextWaveCountdown && !waveNumber &&
                                 !waveCount && !activeSpawnCount && !enemiesCount && !gameSpeed && !gameSimulationEnabled;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            maxLives = true;
            lives = true;
            levelNumber = true;
            energy = true;
            nextWaveCountdown = true;
            waveNumber = true;
            waveCount = true;
            activeSpawnCount = true;
            enemiesCount = true;
            gameSpeed = true;
            gameSimulationEnabled = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            maxLives = false;
            lives = false;
            levelNumber = false;
            energy = false;
            nextWaveCountdown = false;
            waveNumber = false;
            waveCount = false;
            activeSpawnCount = false;
            enemiesCount = false;
            gameSpeed = false;
            gameSimulationEnabled = false;
        }
    }
}
