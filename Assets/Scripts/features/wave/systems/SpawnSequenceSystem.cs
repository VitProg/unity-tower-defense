using System;
using System.Collections.Generic;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy;
using td.features.eventBus;
using td.features.level;
using td.features.level.data;
using td.features.state;
using td.features.wave.bus;
using td.utils;
using UnityEngine;

namespace td.features.wave.systems
{
    public class SpawnSequenceSystem : IProtoRunSystem
    {
        [DI] private State state;
        [DI] private LevelMap levelMap;
        [DI] private EventBus events;
        [DI] private Enemy_Service enemyService;

        public void Run()
        {
            var evPool = events.global.GetPool<Wave_SpawnSequence>();
            var evIt = events.global.It<Wave_SpawnSequence>();
            
            foreach (var evEntity in evIt)
            {
                ref var spawnData = ref evPool.Get(evEntity); 
            
                if (spawnData.started == false)
                {
                    spawnData.delayBeforeCountdown -= Time.deltaTime * state.GetGameSpeed();

                    if (spawnData.delayBeforeCountdown > Constants.ZeroFloat) continue;
                    
                    StartSpawnProccess(ref spawnData);
                }
                Tick(ref spawnData, evEntity);
            }
        }

        private static void StartSpawnProccess(ref Wave_SpawnSequence spawnData)
        {
            spawnData.delayBeforeCountdown = 0f;
            spawnData.delayBetweenCountdown = 0f;
            spawnData.started = true;
        }

        private void Tick(ref Wave_SpawnSequence data, int evEntity)
        {
            data.delayBetweenCountdown -= Time.deltaTime * state.GetGameSpeed();

            if (!(data.delayBetweenCountdown <= Constants.ZeroFloat)) return;

            data.delayBetweenCountdown = data.config.delayBetween;
            data.enemyCounter++;

            if (data.enemyCounter > data.config.quantity)
            {
                events.global.Del(evEntity);
                events.unique.GetOrAdd<Event_SpawnSequence_Finished>();
                return;
            }

            var spawners = new List<int>();
            if (data.config.spawner < 0)
            {
                for (var spawnIndex = 0; spawnIndex < levelMap.SpawnCount; spawnIndex++)
                    spawners.Add(spawnIndex);
            }
            else if (!levelMap.HasSpawn(data.config.spawner))
            {
                spawners.Add((data.lastSpawner + 1) % levelMap.SpawnCount);
            }
            else
            {
                spawners.Add(data.config.spawner);
            }

            foreach (var spawner in spawners)
            {
                var spawnedEnemy = GetNextEnemy(ref data);
                
                enemyService.SpawnEnemy(
                    enemyName: spawnedEnemy.name,
                    enemyType: spawnedEnemy.type,
                    enemyVariant: spawnedEnemy.variant,
                    new SpawnData
                    {
                        number = spawner,
                        health = data.config.health,
                        damage = data.config.damage,
                        speed = data.config.speed,
                        offsetMin = FloatUtils.DefaultIfZero(data.config.offset?[0]),
                        offsetMax = FloatUtils.DefaultIfZero(data.config.offset?[1]),
                        scaleMin = FloatUtils.DefaultIfZero(data.config.scale?[0]),
                        scaleMax = FloatUtils.DefaultIfZero(data.config.scale?[1]),
                    }
                );

                data.lastSpawner = spawner;
            }
        }

        private LevelConfig.WaveSpawnConfigEnemy GetNextEnemy(ref Wave_SpawnSequence spawnData)
        {
            var selectMethod = spawnData.config.selectMethod;

            var spawnedEnemy = selectMethod == LevelConfig.MethodOfSelectNextEnemy.Random
                ? RandomUtils.RandomArrayItem(ref spawnData.config.enemies)
                : spawnData.config.enemies[spawnData.enemyCounter % spawnData.config.enemies.Length]; // todo

            // var enemy = shared.GetEnemyConfig(spawnedEnemy.name);
            var enemy = enemyService.GetEnemyConfig(spawnedEnemy.name);

#if UNITY_EDITOR
            if (enemy == null)
            {
                throw new NullReferenceException($"Enemy with name '{spawnedEnemy.name}' not found on enemies config.");
            }
#endif

            return spawnedEnemy;
        }
    }
}