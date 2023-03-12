using System;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.common.levelEvents;
using td.components.commands;
using td.components.waves;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.waves
{
    public class SpawnSequenceSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsSharedInject<SharedData> shared = default;
        private readonly EcsWorldInject world = default;
        private readonly EcsWorldInject eventsWorld = Constants.Ecs.EventWorldName;
        
        private readonly EcsFilterInject<Inc<SpawnSequence>> entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var spawnData = ref entities.Pools.Inc1.Get(entity);

                if (spawnData.Started == false)
                {
                    spawnData.DelayBeforeCountdown -= Time.deltaTime;

                    if (spawnData.DelayBeforeCountdown < 0.0001f)
                    {
                        StartSpawnProccess(ref spawnData);
                        Tick(ref spawnData, entity);
                    }
                }
                else
                {
                    Tick(ref spawnData, entity);
                }
            }
        }

        private void StartSpawnProccess(ref SpawnSequence spawnData)
        {
            spawnData.DelayBeforeCountdown = 0f;
            spawnData.DelayBetweenCountdown = 0f;
            spawnData.Started = true;
        }

        private SpawnSequence Tick(ref SpawnSequence spawnData, int entity)
        {
            spawnData.DelayBetweenCountdown -= Time.deltaTime;

            if (!(spawnData.DelayBetweenCountdown <= 0.0001f)) return spawnData;
            
            spawnData.DelayBetweenCountdown = spawnData.Config.delayBetween;
            spawnData.EnemyCounter++;
                
            if (spawnData.EnemyCounter > spawnData.Config.quantity)
            {
                Finish(entity);
                return spawnData;
            }
            
            var enemyConfig = GetNextEnemy(ref spawnData);
            EcsEventUtils.Send(eventsWorld.Value, new SpawnEnemyCommand()
            {
                enemyName = enemyConfig.name,
                spawner = spawnData.Config.spawner,
                speed = enemyConfig.baseSpeed * spawnData.Config.speed,
                health = enemyConfig.baseHealth * spawnData.Config.health,
                damage = enemyConfig.baseDamage * spawnData.Config.damage,
                angularSpeed = enemyConfig.angularSpeed,
            });

            return spawnData;
        }

        private EnemyConfig GetNextEnemy(ref SpawnSequence spawnData)
        {
            var selectMethod = spawnData.Config.selectMethod;

            var needEnemyName = selectMethod == SelectEnemyTypeMethod.Random
                ? RandomUtils.RandomArrayItem(spawnData.Config.enemies)
                : spawnData.Config.enemies[spawnData.EnemyCounter % spawnData.Config.enemies.Length];

            var enemy = shared.Value.EnemyConfigs.SingleOrDefault(e => e.name == needEnemyName);

            if (enemy.name == string.Empty)
            {
                throw new NullReferenceException($"Enemy with name '{needEnemyName}' not found on enemies config.");
            }

            return enemy;
        }

        private void Finish(int entity)
        {
            world.Value.DelEntity(entity);
            EcsEventUtils.Send<SpawnSequenceFinishedEvent>(eventsWorld.Value);
        }
    }
}