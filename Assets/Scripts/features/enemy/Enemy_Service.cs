using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.enemy.bus;
using td.features.enemy.components;
using td.features.enemy.data;
using td.features.enemy.mb;
using td.features.goPool;
using td.features.level;
using td.features.state;
using td.monoBehaviours;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.enemy
{
    public class Enemy_Service
    {
        private readonly EcsInject<Enemy_Pools> pools;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<SharedData> shared;
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<GameObjectPool_Service> goPoolService;
        private readonly EcsInject<EnemyPath_Service> enemyPathService;
        private readonly EcsInject<Enemy_Converter> converter;
        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsWorldInject world;

        public bool HasEnemy(EcsPackedEntity packedEntity, out int enemyEntity) => packedEntity.Unpack(world.Value, out enemyEntity) && pools.Value.enemyPool.Value.Has(enemyEntity);
        public bool HasEnemy(EcsPackedEntityWithWorld packedEntity, out int enemyEntity) => packedEntity.Unpack(out _, out enemyEntity) && pools.Value.enemyPool.Value.Has(enemyEntity);
        public bool HasEnemy(int enemyEntity) => pools.Value.enemyPool.Value.Has(enemyEntity);

        public ref Enemy GetEnemy(EcsPackedEntity packedEntity)
        {
            if (!HasEnemy(packedEntity, out var enemyEntity)) throw new NullReferenceException("Entity don't have Enemy component. Use HasEnemy method before");
            return ref GetEnemy(enemyEntity);
        }
        public ref Enemy GetEnemy(EcsPackedEntityWithWorld packedEntity)
        {
            if (!HasEnemy(packedEntity, out var enemyEntity)) throw new NullReferenceException("Entity don't have Enemy component. Use HasEnemy method before");
            return ref GetEnemy(enemyEntity);
        }
        public ref Enemy GetEnemy(int enemyEntity) => ref pools.Value.enemyPool.Value.GetOrAdd(enemyEntity);

        public ref Ref<EnemyMonoBehaviour> GetEnemyMBRef(int enemyEntity) => ref pools.Value.enemyRefMBPool.Value.GetOrAdd(enemyEntity);
        public EnemyMonoBehaviour GetEnemyMB(int enemyEntity) => GetEnemyMBRef(enemyEntity).reference!;

        public bool IsDead(int enemyEntity) => pools.Value.isEnemyDeadPool.Value.Has(enemyEntity);
        public void SetIsDead(int enemyEntity, bool value) => pools.Value.isEnemyDeadPool.Value.SetExistence(enemyEntity, value);
        
        public void Kill(int enemyEntity)
        {
            common.Value.SetIsDestroyed(enemyEntity, true);
            SetIsDead(enemyEntity, true);
        }
        
        public int GetEnemiesCount() => pools.Value.livingEnemiesFilter.Value.GetEntitiesCount();

        public bool IsAlive(EcsPackedEntity enemyPackedEntity, out int enemyEntity) =>
            enemyPackedEntity.Unpack(world.Value, out enemyEntity) && IsAlive(enemyEntity);

        public bool IsAlive(EcsPackedEntityWithWorld enemyPackedEntity, out int enemyEntity) =>
            enemyPackedEntity.Unpack(out _, out enemyEntity) && IsAlive(enemyEntity);

        public bool IsAlive(int enemyEntity) =>
            HasEnemy(enemyEntity) &&
            !IsDead(enemyEntity) &&
            !common.Value.IsDestroyed(enemyEntity) &&
            !common.Value.IsDisabled(enemyEntity) &&
            common.Value.HasGameObject(enemyEntity, true, true);

        private static float O(float value) => FloatUtils.DefaultIfZero(value, 1f);
        
        public int SpawnEnemy(
            string enemyName,
            int enemyType,
            int enemyVariant,
            SpawnData spawnData
        )
        {
            var enemyConfig = shared.Value.GetEnemyConfig(enemyName);
            EnemyConfigType? typedConfig = null;
            
            if (enemyConfig == null) return -1;

            var spawnCoords = levelMap.Value.GetSpawn(spawnData.number);
            
            // enemyPathService.Value.PrepareEnemyPath(ref spawnCoords, enemyEntity);
            var pathNumber = enemyPathService.Value.RandomPathNumber(ref spawnCoords);
            var path = enemyPathService.Value.GetPath(ref spawnCoords, pathNumber);
            
            // минимальный маршрут 3 клетки!!!
            if (path.Count < 3) return -1;

            var pathItem1 = path[0];
            var pathItem2 = path[1];
            var pathItem3 = path[2];

            ref var spawnCell = ref levelMap.Value.GetCell(pathItem1.x, pathItem1.y);
            ref var nextCell = ref levelMap.Value.GetCell(pathItem2.x, pathItem2.y);
            ref var nextNextCell = ref levelMap.Value.GetCell(pathItem3.x, pathItem3.y);

            if (spawnCell.IsEmpty || nextCell.IsEmpty || nextNextCell.IsEmpty) return -1;
            
            // if (
            //     !levelMap.Value.TryGetCell(pathItem1, out var spawnCell) ||
            //     !levelMap.Value.TryGetCell(pathItem2, out var nextCell) || 
            //     !levelMap.Value.TryGetCell(pathItem3, out var nextNextCell) 
            // ) return -1;

            var nextCoords = nextCell.coords;
            var nextNextCoords = nextNextCell.coords;
            
            var enemyPoolableObject = goPoolService.Value.Get(
                enemyConfig.Value.prefab,
                shared.Value.EnemiesContainer.transform,
                Constants.Pools.EnemyDefaultCopacity, 
                Constants.Pools.EnemyMaxCopacity,
                null,
                null,
                delegate(PoolableObject go)
                {
                    go.gameObject.SetActive(false);
                    if (!go.transform.TryGetComponent(out SpriteRenderer sr))
                    {
                        sr = go.transform.GetComponentInChildren<SpriteRenderer>();
                    }
                    if (sr != null) sr.color = Color.white; 
                },
                ActionOnDestroy
            );
            var enemyEntity = converter.Value.GetEntity(enemyPoolableObject.gameObject) ?? world.Value.NewEntity();
            converter.Value.Convert(enemyPoolableObject.gameObject, enemyEntity);
            
            enemyPathService.Value.SetPath(enemyEntity, ref spawnCoords, pathNumber);
            
            ref var enemy = ref GetEnemy(enemyEntity);
            enemy.enemyName = enemyName;
            enemy.spawmNumber = spawnData.number;
            enemy.health = O(spawnData.health);
            enemy.speed = O(spawnData.speed);
            enemy.damage = O(spawnData.damage);
            enemy.angularSpeed = 100f;

            ref var transform = ref common.Value.GetTransform(enemyEntity);
            transform.ClearChangedStatus();
            
            // switch (enemyName.ToLower())
            // {
            //     case "creep":
            //     {
            //         var type = CreepEnemyMonoBehaviour.ParseType(enemyType);
            //
            //         if (enemyConfig.Value.types.Length > 0)
            //         {
            //             typedConfig = enemyConfig.Value.types[(int)type - 1];
            //             enemy.health *= typedConfig.Value.baseHealth;
            //             enemy.speed *= typedConfig.Value.baseSpeed;
            //             enemy.damage *= typedConfig.Value.baseDamage;
            //             enemy.angularSpeed = typedConfig.Value.angularSpeed;
            //         }
            //         break;
            //     }
            //     default:
            //         enemy.health *= enemyConfig.Value.baseHealth;
            //         enemy.speed *= enemyConfig.Value.baseHealth;
            //         enemy.damage *= enemyConfig.Value.baseDamage;
            //         enemy.angularSpeed = enemyConfig.Value.angularSpeed;
            //         break;
            // }
            // enemy.angularSpeed *= Mathf.Clamp(spawnData.speed / 5f, 1f, 100f);
            ApplySpecificEnemyVariant(
                enemyConfig.Value,
                enemyType,
                enemyVariant,
                enemyPoolableObject.gameObject,
                ref enemy
            );
            enemy.angularSpeed *= Mathf.Clamp(spawnData.speed / 5f, 1f, 100f);
            
            transform.SetScale(RandomUtils.Range(
                spawnData.scaleMin > 0.001f ? spawnData.scaleMin : Constants.Enemy.MinSize,
                spawnData.scaleMax > 0.001f ? spawnData.scaleMax : Constants.Enemy.MaxSize
            )); 
            enemy.offset = RandomUtils.Vector2(
                spawnData.offsetMin > 0.001f ? spawnData.offsetMin : Constants.Enemy.OffsetMin,
                spawnData.offsetMax > 0.001f ? spawnData.offsetMax : Constants.Enemy.OffsetMax
            );
            enemy.energy = (uint)Mathf.Max(
                (enemy.health * enemy.damage * enemy.speed * transform.ScaleScalar) / 5, 
                1
            ); //todo move to utils/helper/service/calculator...

            enemy.offset.x = Mathf.Clamp(enemy.offset.x, Constants.Enemy.OffsetMin, Constants.Enemy.OffsetMax);
            enemy.offset.y = Mathf.Clamp(enemy.offset.y, Constants.Enemy.OffsetMin, Constants.Enemy.OffsetMax);
            // enemy.offset.x = 0;
            // enemy.offset.y = 0;
            
            enemy.startingHealth = enemy.health;
            enemy.startingSpeed = enemy.speed;

            transform.SetRotation(EnemyUtils.LookToNextCell(ref spawnCell, ref nextCell));
            transform.SetPosition(EnemyUtils.CalcPosition(spawnCell.coords, transform.rotation, enemy.offset));
            var goTransform = enemyPoolableObject.transform;
            goTransform.position = transform.position;
            goTransform.localScale = transform.GetScaleVector();

            var enemyMb = GetEnemyMB(enemyEntity);//enemyPoolableObject.GetComponent<EnemyMonoBehaviour>());
            enemyMb.body.transform.rotation = transform.rotation;
            enemyMb.hp.minValue = 0.0f;
            enemyMb.hp.maxValue = enemy.health;
            enemyMb.hp.value = enemy.health;
            enemyMb.hp.gameObject.SetActive(false);
            enemyMb.hpLine.color = Constants.Enemy.HpBarColors[^1];
            
            if (enemyMb.animator != null)
            {
                var animSpeed = typedConfig is { animationSpeed: > 0 }
                    ? typedConfig.Value.animationSpeed
                    : enemyConfig.Value.animationSpeed;
                animSpeed = FloatUtils.DefaultIfZero(animSpeed, 1f);
                enemyMb.baseAnimationSpeed = animSpeed;
                enemyMb.animator!.speed = animSpeed * state.Value.GameSpeed;
            }



            ref var movement = ref common.Value.GetMovement(enemyEntity);
            movement.from = transform.position;
            movement.target = EnemyUtils.CalcPosition(ref nextCoords, transform.rotation, enemy.offset);
            movement.nextTarget = EnemyUtils.CalcPosition(ref nextNextCoords, EnemyUtils.LookToNextCell(ref nextCell, ref nextNextCell), enemy.offset);
            movement.SetSpeed(enemy.speed, transform.rotation);
            movement.gapSqr = Constants.DefaultGapSqr;
            movement.speedOfGameAffected = true;
            
            SetIsDead(enemyEntity, false);

            Debug.Log("New enemy:" + enemy);
            

            state.Value.EnemiesCount++;

            return enemyEntity;
        }

        private void ApplySpecificEnemyVariant(EnemyConfig enemyConfig, int enemyType, int enemyVariant, GameObject gameObject, ref Enemy enemy)
        {
            switch (enemyConfig.name.ToLower())
            {
                case "creep":
                {
                    var creepType = CreepEnemyMonoBehaviour.ParseType(enemyType);
                    var creepVariant = CreepEnemyMonoBehaviour.ParseVariant(enemyVariant);

                    if (enemyConfig.types.Length > 0)
                    {
                        var typedConfig = enemyConfig.types[(int)creepType - 1];
                        enemy.health *= typedConfig.baseHealth;
                        enemy.speed *= typedConfig.baseSpeed;
                        enemy.damage *= typedConfig.baseDamage;
                        enemy.angularSpeed = typedConfig.angularSpeed;
                    }
                    
                    var mb = gameObject.GetComponent<CreepEnemyMonoBehaviour>();

                    mb.type = creepType;
                    mb.variant = creepVariant;
                    mb.UpdateView();
                    
                    break;
                }
                default:
                    enemy.health *= enemyConfig.baseHealth;
                    enemy.speed *= enemyConfig.baseHealth;
                    enemy.damage *= enemyConfig.baseDamage;
                    enemy.angularSpeed = enemyConfig.angularSpeed;
                    break;
                
            }
        }

        private void ActionOnDestroy(PoolableObject o)
        {
            var ecsEntity = o.GetComponent<EcsEntity>();
            if (ecsEntity != null && 
                ecsEntity.packedEntity.HasValue &&
                ecsEntity.packedEntity.Value.Unpack(out _, out var entity)
               ) {
                world.Value.DelEntity(entity);
            }
            Object.Destroy(o.gameObject);
        }

        public void ChangeHealthRelative(int enemyEntity, float helthRelative)
        {
            if (Mathf.Abs(helthRelative) < 0.0001f) return;
            GetEnemy(enemyEntity).health += helthRelative;
            events.Value.Entity.Add<Event_Enemy_ChangeHealth>(enemyEntity, world.Value);
        }        
        
        public void ChangeHealth(int enemyEntity, float health)
        {
            ref var enemy = ref GetEnemy(enemyEntity);
            if (FloatUtils.IsEquals(enemy.health, health)) return;
            enemy.health = health;
            events.Value.Entity.Add<Event_Enemy_ChangeHealth>(enemyEntity, world.Value);
        }

        public void ChangeSpeed(int enemyEntity, float speed)
        {
            ref var enemy = ref GetEnemy(enemyEntity);
            if (FloatUtils.IsEquals(enemy.speed, speed)) return;
            enemy.speed = speed;
            if (common.Value.HasMovement(enemyEntity))
                common.Value.GetMovement(enemyEntity).SetSpeed(enemy.speed);
        }

        public bool FindNearestEnemy(Vector2 position, float sqrRadius, out int nearestEnemyEntity)
        {
            var minSqrDistance = float.MaxValue;
            nearestEnemyEntity = -1;
            foreach (var enemyEntity in pools.Value.livingEnemiesFilter.Value)
            {
                var enemyPosition = pools.Value.livingEnemiesFilter.Pools.Inc2.Get(enemyEntity).position;
                
                var dx = enemyPosition.x - position.x;
                var dy = enemyPosition.y - position.y;
                var sqrDx = dx * dx;
                var sqrDy = dy * dy;

                if (sqrDx > sqrRadius || sqrDy > sqrRadius) continue;

                var toEnemy = enemyPosition - position;
                var sqrDistanse = toEnemy.x * toEnemy.x + toEnemy.y * toEnemy.y;
                
                if (sqrDistanse > sqrRadius) continue;
                if (minSqrDistance < sqrDistanse) continue;
                
                minSqrDistance = sqrDistanse;
                nearestEnemyEntity = enemyEntity;
            }

            return nearestEnemyEntity > -1;
        }
    }

    public struct SpawnData
    {
        public int number;
        public float health;
        public float speed;
        public float damage;
        public float scaleMin;
        public float scaleMax;
        public float offsetMin;
        public float offsetMax;
    }
}