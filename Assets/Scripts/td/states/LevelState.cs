using System;
using Leopotam.EcsLite;
using td.features.input;
using td.features.ui;
using td.utils.ecs;

namespace td.states
{
    [Serializable]
    public sealed class LevelState
    {
        private readonly IEcsSystems systems;

        private float maxLives;
        private float lives;
        private int levelNumber;
        private int money;
        private int nextWaveCountdown;
        private int waveNumber;
        private int waveCount;
        private int enemiesCount;
        private bool isLastWave;

        public LevelState(IEcsSystems systems, int levelNumber)
        {
            this.systems = systems;
            this.levelNumber = levelNumber;
        }

        public float MaxLives
        {
            get => maxLives;
            set
            {
                if (Math.Abs(maxLives - value) < Constants.ZeroFloat) return;
                maxLives = value;
                systems.SendOuter(new UpdateUIOuterCommand { MaxLives = value });
            }
        }

        public float Lives
        {
            get => lives;
            set
            {
                if (!(Math.Abs(lives - value) > Constants.ZeroFloat)) return;
                lives = value;
                systems.SendOuter(new UpdateUIOuterCommand { Lives = value });
            }
        }

        public int LevelNumber
        {
            get => levelNumber;
            set
            {
                if (levelNumber == value) return;
                levelNumber = value;
                systems.SendOuter(new UpdateUIOuterCommand { LevelNumber = value });
            }
        }

        public int Money
        {
            get => money;
            set
            {
                if (money == value) return;
                money = value;
                systems.SendOuter(new UpdateUIOuterCommand { Money = value });
            }
        }

        public int NextWaveCountdown
        {
            get => nextWaveCountdown;
            set
            {
                if (nextWaveCountdown == value) return;
                nextWaveCountdown = value;
                systems.SendOuter(new UpdateUIOuterCommand { NextWaveCountdown = value });
            }
        }

        public int WaveNumber
        {
            get => waveNumber;
            set
            {
                if (waveNumber == value) return;
                waveNumber = value;
                systems.SendOuter(new UpdateUIOuterCommand { wave = new[] { waveNumber, waveCount } });

                if (waveNumber > 0 && waveCount > 0 && waveNumber == WaveCount)
                {
                    systems.SendOuter(new UpdateUIOuterCommand { IsLastWave = true });
                }
            }
        }

        public int WaveCount
        {
            get => waveCount;
            set
            {
                if (waveCount == value) return;
                waveCount = value;
                systems.SendOuter(new UpdateUIOuterCommand { wave = new[] { waveNumber, waveCount } });
            }
        }

        public int EnemiesCount
        {
            get => enemiesCount;
            set
            {
                if (enemiesCount == value) return;
                enemiesCount = value;
                systems.SendOuter(new UpdateUIOuterCommand { EnemiesCount = value });
            }
        }

        public bool IsLastWave => waveNumber > 0 && waveCount > 0 && waveNumber == WaveCount;

        private bool isBuildingProcess;
        public bool IsBuildingProcess
        {
            get => isBuildingProcess;
            set
            {
                isBuildingProcess = value;
                systems.SendOuter(new BuildingProcess() { enabled = value });
            }
        }

    }
}