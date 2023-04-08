using System;
using JetBrains.Annotations;

namespace td.features.ui
{
    [Serializable]
    public struct UpdateUIOuterCommand
    {
        public float? MaxLives;
        public float? Lives;
        public int? LevelNumber;
        public int? Money;
        public int? NextWaveCountdown;
        public int? EnemiesCount;
        [CanBeNull] public int[] wave;
        public bool? IsLastWave;
    }
}