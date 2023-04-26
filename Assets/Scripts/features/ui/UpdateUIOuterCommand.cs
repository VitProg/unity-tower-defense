using System;
using JetBrains.Annotations;
using td.services;

namespace td.features.ui
{
    [Serializable]
    public struct UpdateUIOuterCommand
    {
        public float? MaxLives;
        public float? Lives;
        public uint? LevelNumber;
        public int? Money;
        public int? NextWaveCountdown;
        public int? EnemiesCount;
        public int? WaveNumber;
        public int? WaveCount;
        public bool? IsLastWave;

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