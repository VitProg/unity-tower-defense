using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.common.level;
using td.features.enemies;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.waves
{
    public class SpawnSequenceSystem : IEcsRunSystem
    {
        [EcsShared] private SharedData shared;
        [EcsWorld] private EcsWorld world;
        [EcsWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

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

            var enemyConfig = GetNextEnemy(ref spawnData);
            var spawnConfig = new SpawnEnemyOuterCommand()
            {
                enemyName = enemyConfig.name,
                spawner = spawnData.config.spawner,
                speed = enemyConfig.baseSpeed * FloatUtils.DefaultIfZero(spawnData.config.speed, 1f),
                health = enemyConfig.baseHealth * FloatUtils.DefaultIfZero(spawnData.config.health, 1f),
                damage = enemyConfig.baseDamage * FloatUtils.DefaultIfZero(spawnData.config.damage, 1f),
                angularSpeed = enemyConfig.angularSpeed,
                scale = RandomUtils.Range(spawnData.config.scale ?? new[] { Constants.Enemy.MinSize, Constants.Enemy.MaxSize }),
                offset = RandomUtils.Vector2(spawnData.config.offset ?? new[] { Constants.Enemy.OffsetMin, Constants.Enemy.OffsetMax }),
            };
            spawnConfig.money = Math.Max(
                (int)(spawnConfig.health * spawnConfig.damage * spawnConfig.speed * spawnConfig.scale) / 5,
                1
            );
            systems.SendOuter(spawnConfig);

            return spawnData;
        }

        private EnemyConfig GetNextEnemy(ref SpawnSequence spawnData)
        {
            var selectMethod = spawnData.config.selectMethod;

            var needEnemyName = selectMethod == MethodOfSelectNextEnemy.Random
                ? RandomUtils.RandomArrayItem(spawnData.config.enemies)
                : spawnData.config.enemies[spawnData.enemyCounter % spawnData.config.enemies.Length]; // todo

            var enemy = shared.GetEnemyConfig(needEnemyName);

            if (enemy.name == string.Empty)
            {
                throw new NullReferenceException($"Enemy with name '{needEnemyName}' not found on enemies config.");
            }

            return enemy;
        }

        private void Finish(int entity, IEcsSystems systems)
        {
            world.DelEntity(entity);
            systems.SendOuter<SpawnSequenceFinishedOuterEvent>();
        }
    }
}