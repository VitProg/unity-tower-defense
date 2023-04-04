using System;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.behaviors;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies
{
    public class EnemyTestSpawnerExecutor : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsPoolInject<MoveToTarget> moveToTargetPointPool = default;

        private static float TimeBetweenSpawns = 2f;
        private static float TimeBetweenSpawnsStep = 0.1f;
        private static float MinTimeBetweenSpawns = 0.001f;
        private static float timeFromLastSpawn = TimeBetweenSpawns;
        private static float waveSpeed = 1f;

        public void Run(IEcsSystems systems)
        {
            timeFromLastSpawn += Time.deltaTime;
            if (timeFromLastSpawn < TimeBetweenSpawns)
            {
                return;
            }
            timeFromLastSpawn = 0f;
            TimeBetweenSpawns = Math.Max(TimeBetweenSpawns - TimeBetweenSpawnsStep, MinTimeBetweenSpawns);
            waveSpeed += 0.0001f;
            
            var world = systems.GetWorld();
            var sharedData = systems.GetShared<SharedData>();

            var waveNumber = 1;
            var levelNumber = levelData.Value.LevelNumber;
            
            for (var spawnIndex = 0; spawnIndex < 4; spawnIndex++)
            {
                var waveConfig = levelData.Value.LevelConfig?.waves[waveNumber - 1];
                var spawmingConfig = waveConfig?.spawns[0];
                
                var enemyConfig = sharedData.GetEnemyConfig(spawmingConfig?.enemies[0]);

                EcsEventUtils.Send(systems, new SpawnEnemyCommand()
                {
                    enemyName = enemyConfig.name,
                    speed = (enemyConfig.baseSpeed * waveSpeed * levelNumber),
                    health = (enemyConfig.baseHealth * (waveSpeed / 10f) * levelNumber),
                    angularSpeed = enemyConfig.angularSpeed,
                    spawner = spawnIndex,
                });
            }
        }
    }
}