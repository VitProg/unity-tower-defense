using System;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.behaviors;
using td.services;
using td.states;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies
{
    public class EnemyTestSpawnerExecutor : IEcsRunSystem
    {
        [EcsInject] private LevelMap levelMap;
        [EcsInject] private LevelState levelState;

        private static float _timeBetweenSpawns = 2f;
        private const float TimeBetweenSpawnsStep = 0.1f;
        private const float MinTimeBetweenSpawns = 0.001f;
        private static float _timeFromLastSpawn = _timeBetweenSpawns;
        private static float _waveSpeed = 1f;

        public void Run(IEcsSystems systems)
        {
            _timeFromLastSpawn += Time.deltaTime;
            if (_timeFromLastSpawn < _timeBetweenSpawns)
            {
                return;
            }
            _timeFromLastSpawn = 0f;
            _timeBetweenSpawns = Math.Max(_timeBetweenSpawns - TimeBetweenSpawnsStep, MinTimeBetweenSpawns);
            _waveSpeed += 0.0001f;
            
            var world = systems.GetWorld();
            var sharedData = systems.GetShared<SharedData>();

            var waveNumber = 1;
            var levelNumber = levelState.LevelNumber;
            
            for (var spawnIndex = 0; spawnIndex < 4; spawnIndex++)
            {
                var waveConfig = levelMap.LevelConfig?.waves[waveNumber - 1];
                var spawmingConfig = waveConfig?.spawns[0];
                
                var enemyConfig = sharedData.GetEnemyConfig(spawmingConfig?.enemies[0]);

                systems.SendOuter(new SpawnEnemyOuterCommand()
                {
                    enemyName = enemyConfig.name,
                    speed = (enemyConfig.baseSpeed * _waveSpeed * levelNumber),
                    health = (enemyConfig.baseHealth * (_waveSpeed / 10f) * levelNumber),
                    angularSpeed = enemyConfig.angularSpeed,
                    spawner = spawnIndex,
                });
            }
        }
    }
}