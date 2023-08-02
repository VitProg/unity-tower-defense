using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.enemy;
using td.features.level;
using td.features.level.data;
using td.features.state;
using td.features.wave.bus;
using td.utils;
using UnityEngine;

namespace td.features.wave.systems
{
    public class SpawnSequenceSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<SharedData> shared;
        private readonly EcsWorldInject world;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Enemy_Service> enemyService;
        private EcsFilter filter;
        private EcsPool<Wave_SpawnSequence> pool;

        public void Run(IEcsSystems systems)
        {
            if (filter.GetEntitiesCount() <= 0) return;

            foreach (var entity in filter)
            {
                ref var spawnData = ref pool.Get(entity); 
            
                if (spawnData.started == false)
                {
                    spawnData.delayBeforeCountdown -= Time.deltaTime * state.Value.GameSpeed;

                    if (spawnData.delayBeforeCountdown > Constants.ZeroFloat) continue;
                    
                    StartSpawnProccess(ref spawnData);
                }
                Tick(ref spawnData, entity);
            }
        }

        private static void StartSpawnProccess(ref Wave_SpawnSequence spawnData)
        {
            spawnData.delayBeforeCountdown = 0f;
            spawnData.delayBetweenCountdown = 0f;
            spawnData.started = true;
        }

        private void Tick(ref Wave_SpawnSequence data, int entity)
        {
            data.delayBetweenCountdown -= Time.deltaTime * state.Value.GameSpeed;

            if (!(data.delayBetweenCountdown <= Constants.ZeroFloat)) return;

            data.delayBetweenCountdown = data.config.delayBetween;
            data.enemyCounter++;

            if (data.enemyCounter > data.config.quantity)
            {
                events.Value.GetEventsWorld().DelEntity(entity);
                events.Value.Unique.Add<Event_SpawnSequence_Finished>();
                return;
            }

            var spawners = new List<int>();
            if (data.config.spawner < 0)
            {
                for (var spawnIndex = 0; spawnIndex < levelMap.Value.SpawnCount; spawnIndex++)
                    spawners.Add(spawnIndex);
            }
            else if (!levelMap.Value.HasSpawn(data.config.spawner))
            {
                spawners.Add((data.lastSpawner + 1) % levelMap.Value.SpawnCount);
            }
            else
            {
                spawners.Add(data.config.spawner);
            }

            foreach (var spawner in spawners)
            {
                var spawnedEnemy = GetNextEnemy(ref data);
                
                enemyService.Value.SpawnEnemy(
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
                ? RandomUtils.RandomArrayItem(spawnData.config.enemies)
                : spawnData.config.enemies[spawnData.enemyCounter % spawnData.config.enemies.Length]; // todo

            var enemy = shared.Value.GetEnemyConfig(spawnedEnemy.name);

            if (enemy == null)
            {
                throw new NullReferenceException($"Enemy with name '{spawnedEnemy.name}' not found on enemies config.");
            }

            return spawnedEnemy;
        }

        public void Init(IEcsSystems systems)
        {
            pool = events.Value.Global.GetPool<Wave_SpawnSequence>();
            filter = events.Value.Global.GetFilter<Wave_SpawnSequence>();
        }
    }
}