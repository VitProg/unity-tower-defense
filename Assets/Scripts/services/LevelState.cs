using System;
using Leopotam.EcsLite;
using td.features.ui;
using td.utils.ecs;

namespace td.services
{
    [Serializable]
    public sealed class LevelState
    {
        [InjectSystems] private IEcsSystems systems = default;

        private float maxLives;
        private float lives;
        private uint levelNumber;
        private int money;
        private int nextWaveCountdown;
        private int waveNumber;
        private int waveCount;
        private int enemiesCount;

        public LevelState(uint levelNumber)
        {
            this.levelNumber = levelNumber;
        }

        public float MaxLives
        {
            get => maxLives;
            set
            {
                if (Math.Abs(maxLives - value) < Constants.ZeroFloat) return;
                maxLives = value;
                systems.Outer<UpdateUIOuterCommand>().MaxLives = value;
            }
        }

        public float Lives
        {
            get => lives;
            set
            {
                if (!(Math.Abs(lives - value) > Constants.ZeroFloat)) return;
                lives = value;
                systems.Outer<UpdateUIOuterCommand>().Lives = value;
            }
        }

        public uint LevelNumber
        {
            get => levelNumber;
            set
            {
                if (levelNumber == value) return;
                levelNumber = value;
                systems.Outer<UpdateUIOuterCommand>().LevelNumber = value;
            }
        }

        public int Money
        {
            get => money;
            set
            {
                if (money == value) return;
                money = value;
                systems.Outer<UpdateUIOuterCommand>().Money = value;
            }
        }

        public int NextWaveCountdown
        {
            get => nextWaveCountdown;
            set
            {
                if (nextWaveCountdown == value) return;
                nextWaveCountdown = value;
                systems.Outer<UpdateUIOuterCommand>().NextWaveCountdown = value;
            }
        }

        public int WaveNumber
        {
            get => waveNumber;
            set
            {
                if (waveNumber == value) return;
                waveNumber = value;
                ref var updateUI = ref systems.Outer<UpdateUIOuterCommand>();
                updateUI.WaveNumber = waveNumber;
                updateUI.WaveCount = waveCount;
                updateUI.IsLastWave = waveNumber > 0 && waveCount > 0 && waveNumber == WaveCount;
            }
        }

        public int WaveCount
        {
            get => waveCount;
            set
            {
                if (waveCount == value) return;
                waveCount = value;
                ref var updateUI = ref systems.Outer<UpdateUIOuterCommand>();
                updateUI.WaveNumber = waveNumber;
                updateUI.WaveCount = waveCount;
                updateUI.IsLastWave = waveNumber > 0 && waveCount > 0 && waveNumber == WaveCount;
            }
        }

        public int EnemiesCount
        {
            get => enemiesCount;
            set
            {
                if (enemiesCount == value) return;
                enemiesCount = value;
                systems.Outer<UpdateUIOuterCommand>().EnemiesCount = value;
            }
        }

        public bool IsLastWave => waveNumber > 0 && waveCount > 0 && waveNumber == WaveCount;

        private bool isBuildingProcess;
        public bool IsBuildingProcess
        {
            get => isBuildingProcess;
            set
            {
                if (isBuildingProcess == value) return;
                isBuildingProcess = value;
                // systems.Outer(new BuildingProcess() { enabled = value });
            }
        }

        public void ClearForNewLevel()
        {
            EnemiesCount = 0;
            WaveCount = 0;
            WaveNumber = 0;
            IsBuildingProcess = false;
            nextWaveCountdown = 0;
        }
    }
}