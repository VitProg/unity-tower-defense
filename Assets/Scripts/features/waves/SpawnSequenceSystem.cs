using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.common.level;
using td.features.enemies;
using td.features.enemies.components;
using td.features.enemies.mb;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using EnemyUtils = td.features.enemies.EnemyUtils;

namespace td.features.waves
{
    public class SpawnSequenceSystem : IEcsRunSystem
    {
        [Inject] private LevelMap levelMap;
        [InjectShared] private SharedData shared;
        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        private readonly EcsFilterInject<Inc<SpawnSequence>> entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var spawnData = ref entities.Pools.Inc1.Get(entity);

                if (spawnData.started == false)
                {
                    spawnData.delayBeforeCountdown -= Time.deltaTime;

                    if (spawnData.delayBeforeCountdown < Constants.ZeroFloat)
                    {
                        StartSpawnProccess(ref spawnData);
                        Tick(ref spawnData, entity, systems);
                    }
                }
                else
                {
                    Tick(ref spawnData, entity, systems);
                }
            }
        }

        private void StartSpawnProccess(ref SpawnSequence spawnData)
        {
            spawnData.delayBeforeCountdown = 0f;
            spawnData.delayBetweenCountdown = 0f;
            spawnData.started = true;
        }

        private SpawnSequence Tick(ref SpawnSequence spawnData, int entity, IEcsSystems systems)
        {
            spawnData.delayBetweenCountdown -= Time.deltaTime;

            if (!(spawnData.delayBetweenCountdown <= Constants.ZeroFloat)) return spawnData;

            spawnData.delayBetweenCountdown = spawnData.config.delayBetween;
            spawnData.enemyCounter++;

            if (spawnData.enemyCounter > spawnData.config.quantity)
            {
                Finish(entity, systems);
                return spawnData;
            }

            var spawners = new List<int>();
            if (spawnData.config.spawner < 0)
            {
                spawners.AddRange(levelMap.SpawnsIndexse);
            }
            else if (!levelMap.HasSpawn(spawnData.config.spawner))
            {
                spawners.Add((spawnData.lastSpawner + 1) % levelMap.Spawns.Length);
            }
            else
            {
                spawners.Add(spawnData.config.spawner);
            }

            foreach (var spawner in spawners)
            {
                var (enemyConfig, spawnedEnemy) = GetNextEnemy(ref spawnData);

                ref var spawnCommand = ref systems.Outer<SpawnEnemyOuterCommand>();
                spawnCommand.enemyName = enemyConfig.name;
                spawnCommand.enemyType = spawnedEnemy.type;
                spawnCommand.enemyVariant = spawnedEnemy.variant;
                spawnCommand.spawner = spawner;
                spawnCommand.speed = /*enemyConfig.baseSpeed * */FloatUtils.DefaultIfZero(spawnData.config.speed, 1f);
                spawnCommand.health = /*enemyConfig.baseHealth * */FloatUtils.DefaultIfZero(spawnData.config.health, 1f);
                spawnCommand.damage = /*enemyConfig.baseDamage * */FloatUtils.DefaultIfZero(spawnData.config.damage, 1f);
                spawnCommand.angularSpeed = enemyConfig.angularSpeed;
                spawnCommand.scale = RandomUtils.Range(spawnData.config.scale ?? new[] { Constants.Enemy.MinSize, Constants.Enemy.MaxSize });
                spawnCommand.offset = RandomUtils.Vector2(spawnData.config.offset ?? new[] { Constants.Enemy.OffsetMin, Constants.Enemy.OffsetMax });
                spawnCommand.offset.x = Mathf.Clamp(spawnCommand.offset.x, Constants.Enemy.OffsetMin, Constants.Enemy.OffsetMax);
                spawnCommand.offset.y = Mathf.Clamp(spawnCommand.offset.y, Constants.Enemy.OffsetMin, Constants.Enemy.OffsetMax);
                spawnCommand.money = Math.Max((int)(spawnCommand.health * spawnCommand.damage * spawnCommand.speed * spawnCommand.scale) / 5, 1);
                
                if (!EnemySpawnUtils.PrepareSpawnCommand(enemyConfig, ref spawnedEnemy, ref spawnCommand))
                {
                    spawnCommand.speed *= enemyConfig.baseSpeed;
                    spawnCommand.health *= enemyConfig.baseHealth;
                    spawnCommand.damage *= enemyConfig.baseDamage;
                }

                spawnData.lastSpawner = spawner;
            }

            return spawnData;
        }

        private (EnemyConfig Value, WaveSpawnConfigEnemy spawnedEnemy) GetNextEnemy(ref SpawnSequence spawnData)
        {
            var selectMethod = spawnData.config.selectMethod;

            var spawnedEnemy = selectMethod == MethodOfSelectNextEnemy.Random
                ? RandomUtils.RandomArrayItem(spawnData.config.enemies)
                : spawnData.config.enemies[spawnData.enemyCounter % spawnData.config.enemies.Length]; // todo

            var enemy = shared.GetEnemyConfig(spawnedEnemy.name);

            if (enemy == null)
            {
                throw new NullReferenceException($"Enemy with name '{spawnedEnemy.name}' not found on enemies config.");
            }

            return (enemy.Value, spawnedEnemy);
        }
    
        private void Finish(int entity, IEcsSystems systems)
        {
            world.DelEntity(entity);
            systems.Outer<SpawnSequenceFinishedOuterEvent>();
        }
    }
}