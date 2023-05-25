using System;
using JetBrains.Annotations;
using td.services;

namespace td.features.ui
{
    [Serializable]
    public struct UpdateUIOuterCommand
    {
        public float? maxLives;
        public float? lives;
        public uint? levelNumber;
        public int? money;
        public float? nextWaveCountdown;
        public int? enemiesCount;
        public int? waveNumber;
        public int? waveCount;
        public bool? isLastWave;

        // public static UpdateUIOuterCommand FromLevelState(LevelState levelState) =>
        //     new()
        //     {
        //         Lives = levelState.Lives,
        //         MaxLives = levelState.MaxLives,
        //         Money = levelState.Money,
        //         LevelNumber = levelState.LevelNumber,
        //         EnemiesCount = levelState.EnemiesCount,
        //         IsLastWave = levelState.IsLastWave,
        //         NextWaveCountdown = levelState.NextWaveCountdown,
        //         wave = new[] { levelState.WaveNumber, levelState.WaveCount },
        //     };
    }
}