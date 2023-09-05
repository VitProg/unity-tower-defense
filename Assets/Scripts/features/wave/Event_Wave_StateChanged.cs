using System;
using System.Runtime.CompilerServices;
using td.features.state.interfaces;

namespace td.features.wave
{
    [Serializable]
    public struct Event_Wave_StateChanged : IStateChangedEvent
    {
        public bool waiting;
        public bool waveNumber;
        public bool waveCount;
        public bool nextWaveCountdown;
        public bool spawners;
        public bool spawnersCount;
        public bool enemiesCount;
        public bool someSpawnerHasBeenUpdated;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => !waiting && !waveNumber && !waveCount && !nextWaveCountdown && !spawners && !spawnersCount && !enemiesCount && !someSpawnerHasBeenUpdated;
        
        public void Clear()
        {
            waiting = false;
            waveNumber = false;
            waveCount = false;
            nextWaveCountdown = false;
            spawners = false;
            spawnersCount = false;
            enemiesCount = false;
            someSpawnerHasBeenUpdated = false;
        }

        public void All()
        {
            waiting = true;
            waveNumber = true;
            waveCount = true;
            nextWaveCountdown = true;
            spawners = true;
            spawnersCount = true;
            enemiesCount = true;
            someSpawnerHasBeenUpdated = true;
        }
    }
}