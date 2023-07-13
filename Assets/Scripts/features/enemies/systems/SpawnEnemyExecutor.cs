using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.behaviors;
using td.components.flags;
using td.features.enemies.components;
using td.features.enemies.mb;
using td.features.state;
using td.monoBehaviours;
using td.services;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.enemies.systems
{
    public class SpawnEnemyExecutor : IEcsRunSystem, IEcsInitSystem
    {
        [Inject] private GameObjectPoolService poolService;
        [Inject] private LevelMap levelMap;
        [Inject] private State state;
        [Inject] private EntityConverters converters;
        [Inject] private EnemyPathService enemyPathService;
        [InjectShared] private SharedData shared;
        [InjectWorld] private EcsWorld world;

        private readonly EcsFilterInject<Inc<SpawnEnemyOuterCommand>> eventEntities = Constants.Worlds.Outer;
        private GameObject containerForEnemies;

        public void Run(IEcsSystems systems)
        {
            var outerWorld = systems.GetWorld(Constants.Worlds.Outer);

            foreach (var eventEntity in eventEntities.Value)
            {
                ref var spawnCommand = ref eventEntities.Pools.Inc1.Get(eventEntity);
                var enemyConfig = shared.GetEnemyConfig(spawnCommand.enemyName);
                
                if (enemyConfig == null) continue;

                var spawnCoords = levelMap.GetSpawn(spawnCommand.spawner);
                
                if (
                    !levelMap.TryGetCell(spawnCoords, out var spawnCell) ||
                    !levelMap.TryGetCell(spawnCell.GetRandomNextCoords(), out var nextCell)
                ) continue;

                var rotation = EnemyUtils.LookToNextCell(spawnCell, nextCell);
                var position = EnemyUtils.Position(spawnCell.Coords, rotation, spawnCommand.offset);

                // var enemyGameObject = Object.Instantiate(
                    // enemyConfig.Value.prefab,
                    // position,
                    // enemyConfig.Value.prefab.transform.rotation,
                    //todo containerForEnemies.transform
                // );

                var enemyPoolableObject = poolService.Get(
                    enemyConfig.Value.prefab,
                    containerForEnemies.transform,
                    Constants.Pools.EnemyDefaultCopacity, 
                    Constants.Pools.EnemyMaxCopacity,
                    null,
                    null,
                    null,
                    ActionOnDestroy
                );
                
                var enemyMb = enemyPoolableObject.GetComponent<EnemyMonoBehaviour>();
                
                var transform = enemyPoolableObject.transform;
                transform.position = position;
                transform.localScale = Vector2.one;
                
                enemyMb.body.transform.rotation = rotation;

                enemyMb.hp.minValue = 0.0f;
                enemyMb.hp.maxValue = spawnCommand.health;
                enemyMb.hp.value = spawnCommand.health;
                enemyMb.hpLine.color = new Color(1, 1, 0);

                if (!converters.Convert<Enemy>(enemyPoolableObject.gameObject, out var enemyEntity))
                {
                    throw new NullReferenceException($"Failed to convert GameObject {enemyPoolableObject.gameObject.name}");
                }

                EnemySpawnUtils.PrepareGameObject(enemyPoolableObject.gameObject, ref spawnCommand);
                
                ref var enemy = ref world.GetComponent<Enemy>(enemyEntity);
                enemy.position = position;
                enemy.rotation = rotation;
                enemy.enemyName = spawnCommand.enemyName;
                enemy.spawner = spawnCommand.spawner;
                enemy.speed = spawnCommand.speed;
                enemy.startingSpeed = spawnCommand.speed;
                enemy.angularSpeed = spawnCommand.angularSpeed;
                enemy.health = spawnCommand.health;
                enemy.startingHealth = spawnCommand.health;
                enemy.damage = spawnCommand.damage;
                enemy.scale = spawnCommand.scale;
                enemy.offset = spawnCommand.offset;
                enemy.money = spawnCommand.money;

                ref var movement = ref world.GetComponent<LinearMovementToTarget>(enemyEntity);
                movement.from = position;
                movement.target = EnemyUtils.Position(
                    nextCell.Coords,
                    rotation,
                    spawnCommand.offset
                );
                movement.speed = spawnCommand.speed;
                movement.gap = Constants.DefaultGap;
                movement.speedOfGameAffected = true;
                
                world.DelComponent<IsEnemyDead>(enemyEntity);
                world.DelComponent<IsDisabled>(enemyEntity);
                world.DelComponent<IsDestroyed>(enemyEntity);
                
                enemyPathService.PrepareEnemyPath(ref spawnCoords, enemyEntity);

                state.EnemiesCount++;

                outerWorld.DelEntity(eventEntity);
            }
        }
        public void Init(IEcsSystems systems)
        {
            containerForEnemies = GameObject.FindGameObjectWithTag(Constants.Tags.EnemiesContainer);
        }
        
        private void ActionOnDestroy(PoolableObject o)
        {
            var ecsEntity = o.GetComponent<EcsEntity>();
            if (ecsEntity != null && 
                ecsEntity.PackedEntity.HasValue &&
                ecsEntity.PackedEntity.Value.Unpack(world, out var entity)
               ) {
                world.DelEntity(entity);
            }
            Object.Destroy(o.gameObject);
        }
    }
}