using System;
using System.Collections.Generic;
using Leopotam.EcsProto.QoL;
using td.features.enemy;
using td.features.eventBus;
using td.features.level;
using td.features.state;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.wave.systems
{
    public class Wave_SpawnSequence_System : ProtoIntervalableRunSystem
    {
        [DI] private EventBus events;
        [DI] private State state;
        [DI] private Wave_State waveState;
        [DI] private Level_State levelState;
        [DI] private Enemy_Service enemyService;
        
        //
        private readonly List<int> spawnPoints = new (10);
        private SpawnData spawnData;
        //
        
        public override void IntervalRun(float deltaTime)
        {
            if (state.GetGameSpeed() == 0 || !state.GetSimulationEnabled()) return;
            if (waveState.GetWaiting()) return;
            if (waveState.GetActiveSpawnersCount() == 0) return;
            
            var count = waveState.GetSpawnersCount();
            for (var idx = 0; idx < count; idx++)
            {
                ref var spawner = ref waveState.GetSpawner(idx);
                if (spawner.IsFinished()) continue;
                ProcessSpawner(idx, ref spawner, deltaTime);
            }
        }

        private void ProcessSpawner(int idx, ref WaveSpawnSequence spawner, float deltaTime)
        {
            if (spawner.timeRemains > 0f) spawner.timeRemains -= deltaTime * state.GetGameSpeed();
            if (spawner.timeRemains > 0f) return;
            
            if (spawner.IsFinished()) return;
            
            spawner.timeRemains = spawner.config.delayBetween;
            spawner.enemyCounter++;

            waveState.SomeSpawnerHasBeenUpdated();
            
            // определяем все доступные спавн точки для спавнера
            spawnPoints.Clear();
            if (spawner.config.spawner < 0)
            {
                for (var spawnIndex = 0; spawnIndex < levelState.GetSpawnCount(); spawnIndex++)
                    spawnPoints.Add(spawnIndex);
            }
            else if (!levelState.HasSpawn(spawner.config.spawner))
            {
                spawnPoints.Add((spawner.lastSpawnPoint + 1) % levelState.GetSpawnCount());
            }
            else
            {
                spawnPoints.Add(spawner.config.spawner);
            }
            
            foreach (var spawnPoint in spawnPoints)
            {
                ref var spawnedEnemy = ref spawner.GetNextEnemy();
                
                enemyService.SpawnEnemy(
                    enemyName: spawnedEnemy.name,
                    enemyType: spawnedEnemy.type,
                    enemyVariant: spawnedEnemy.variant,
                    spawnPointNumber: spawnPoint,
                    health: spawner.config.health,
                    damage: spawner.config.damage,
                    speed: spawner.config.speed,
                    offsetMin: FloatUtils.DefaultIfZero(spawner.config.offset?[0]),
                    offsetMax: FloatUtils.DefaultIfZero(spawner.config.offset?[1]),
                    scaleMin: FloatUtils.DefaultIfZero(spawner.config.scale?[0]),
                    scaleMax: FloatUtils.DefaultIfZero(spawner.config.scale?[1])
                );

                spawner.lastSpawnPoint = spawnPoint;
            }
            waveState.IncreaseEnemiesCount(spawnPoints.Count);
        }
        
        public Wave_SpawnSequence_System(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}