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
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsSharedInject<SharedData> shared = default;
        private readonly EcsWorldInject world = default;
        private readonly EcsWorldInject eventsWorld = Constants.Ecs.EventsWorldName;

        private readonly EcsFilterInject<Inc<SpawnSequence>> entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var spawnData = ref entities.Pools.Inc1.Get(entity);

                if (spawnData.Started == false)
                {
                    spawnData.DelayBeforeCountdown -= Time.deltaTime;

                    if (spawnData.DelayBeforeCountdown < Constants.ZeroFloat)
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

            if (!(spawnData.DelayBetweenCountdown <= Constants.ZeroFloat)) return spawnData;

            spawnData.DelayBetweenCountdown = spawnData.Config.delayBetween;
            spawnData.EnemyCounter++;

            if (spawnData.EnemyCounter > spawnData.Config.quantity)
            {
                Finish(entity);
                return spawnData;
            }

            var enemyConfig = GetNextEnemy(ref spawnData);
            var spawnConfig = new SpawnEnemyCommand()
            {
                enemyName = enemyConfig.name,
                spawner = spawnData.Config.spawner,
                speed = enemyConfig.baseSpeed * FloatUtils.DefaultIfZero(spawnData.Config.speed, 1f),
                health = enemyConfig.baseHealth * FloatUtils.DefaultIfZero(spawnData.Config.health, 1f),
                damage = enemyConfig.baseDamage * FloatUtils.DefaultIfZero(spawnData.Config.damage, 1f),
                angularSpeed = enemyConfig.angularSpeed,
                scale = RandomUtils.Range(spawnData.Config.scale ?? new[] { Constants.Enemy.MinSize, Constants.Enemy.MaxSize }),
                offset = RandomUtils.Vector2(spawnData.Config.offset ?? new[] { Constants.Enemy.OffsetMin, Constants.Enemy.OffsetMax }),
            };
            spawnConfig.money = Math.Max(
                (int)(spawnConfig.health * spawnConfig.damage * spawnConfig.speed * spawnConfig.scale) / 5,
                1
            );
            EcsEventUtils.Send(eventsWorld.Value, spawnConfig);

            return spawnData;
        }

        private EnemyConfig GetNextEnemy(ref SpawnSequence spawnData)
        {
            var selectMethod = spawnData.Config.selectMethod;

            var needEnemyName = selectMethod == MethodOfSelectNextEnemy.Random
                ? RandomUtils.RandomArrayItem(spawnData.Config.enemies)
                : spawnData.Config.enemies[spawnData.EnemyCounter % spawnData.Config.enemies.Length]; // todo

            var enemy = shared.Value.GetEnemyConfig(needEnemyName);

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