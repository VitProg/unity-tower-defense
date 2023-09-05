using System;
using System.Runtime.CompilerServices;
using td.features._common.data;
using td.features.enemy;
using td.utils.di;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace td.features.wave
{
    [Serializable]
    public struct WaveSpawnSequence
    {
        public LevelConfig.WaveSpawnConfig config;
        public int lastSpawnPoint;
        public int enemyCounter;
        public float timeRemains;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsFinished() => enemyCounter >= config.quantity;

        public ref LevelConfig.WaveSpawnConfigEnemy GetNextEnemy()
        {
            var index = config.selectMethod == LevelConfig.MethodOfSelectNextEnemy.Random
                ? Random.Range(0, config.enemies.Length)
                : enemyCounter % config.enemies.Length;

#if UNITY_EDITOR
            if (ServiceContainer.Get<Enemy_Service>().GetEnemyConfig(config.enemies[index].name) == null)
            {
                throw new NullReferenceException($"Enemy with name '{config.enemies[index].name}' not found on enemies config.");
            }
#endif

            return ref config.enemies[index];
        }
    }
}