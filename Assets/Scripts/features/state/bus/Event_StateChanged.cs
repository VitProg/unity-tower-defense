using System;
using System.Runtime.CompilerServices;
using td.features.state.interfaces;

namespace td.features.state.bus
{
    [Serializable]
    public struct Event_StateChanged : IStateChangedEvent
    {
        public bool maxLives;
        public bool lives;
        public bool levelNumber;
        public bool maxEnergy;
        public bool energy;
        // public bool nextWaveCountdown;
        // public bool waveNumber;
        // public bool waveCount;
        // public bool activeSpawnCount;
        // public bool enemiesCount;
        public bool killsCount;
        public bool gameSpeed;
        public bool simulationEnabled;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !maxLives && !lives && !levelNumber && !maxEnergy && !energy && !killsCount && !gameSpeed && !simulationEnabled;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void All()
        {
            maxLives = true;
            lives = true;
            levelNumber = true;
            maxEnergy = true;
            energy = true;
            // nextWaveCountdown = true;
            // waveNumber = true;
            // waveCount = true;
            // activeSpawnCount = true;
            // enemiesCount = true;
            killsCount = true;
            gameSpeed = true;
            simulationEnabled = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            maxLives = false;
            lives = false;
            levelNumber = false;
            maxEnergy = false;
            energy = false;
            // nextWaveCountdown = false;
            // waveNumber = false;
            // waveCount = false;
            // activeSpawnCount = false;
            // enemiesCount = false;
            killsCount = false;
            gameSpeed = false;
            simulationEnabled = false;
        }
    }
}
