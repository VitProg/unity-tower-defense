using System;
using Leopotam.EcsLite;
using td.features.ui;
using td.utils;
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
        private float nextWaveCountdown;
        private int waveNumber;
        private int waveCount;
        private int enemiesCount;
        private bool isBuildingProcess;

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
                systems.Outer<UpdateUIOuterCommand>().maxLives = value;
            }
        }

        public float Lives
        {
            get => lives;
            set
            {
                if (!(Math.Abs(lives - value) > Constants.ZeroFloat)) return;
                lives = value;
                systems.Outer<UpdateUIOuterCommand>().lives = value;
            }
        }

        public uint LevelNumber
        {
            get => levelNumber;
            set
            {
                if (levelNumber == value) return;
                levelNumber = value;
                systems.Outer<UpdateUIOuterCommand>().levelNumber = value;
            }
        }

        public int Money
        {
            get => money;
            set
            {
                if (money == value) return;
                money = value;
                systems.Outer<UpdateUIOuterCommand>().money = value;
            }
        }

        public float NextWaveCountdown
        {
            get => nextWaveCountdown;
            set
            {
                if (FloatUtils.IsEquals(nextWaveCountdown, value)) return;
                nextWaveCountdown = value;
                systems.Outer<UpdateUIOuterCommand>().nextWaveCountdown = value;
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
                updateUI.waveNumber = waveNumber;
                updateUI.waveCount = waveCount;
                updateUI.isLastWave = waveNumber > 0 && waveCount > 0 && waveNumber == WaveCount;
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
                updateUI.waveNumber = waveNumber;
                updateUI.waveCount = waveCount;
                updateUI.isLastWave = waveNumber > 0 && waveCount > 0 && waveNumber == WaveCount;
            }
        }

        public int EnemiesCount
        {
            get => enemiesCount;
            set
            {
                if (enemiesCount == value) return;
                enemiesCount = value;
                systems.Outer<UpdateUIOuterCommand>().enemiesCount = value;
            }
        }

        public bool IsLastWave => waveNumber > 0 && waveCount > 0 && waveNumber == WaveCount;

        public bool IsBuildingProcess
        {
            get => isBuildingProcess;
            set => isBuildingProcess = value;
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