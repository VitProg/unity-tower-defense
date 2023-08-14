using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.destroy;
using td.features.enemy.bus;
using td.features.enemy.components;
using td.features.enemy.data;
using td.features.enemy.mb;
using td.features.eventBus;
using td.features.goPool;
using td.features.level;
using td.features.movement;
using td.features.state;
using td.monoBehaviours;
using td.utils;
using td.utils.di;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.enemy
{
    public class Enemy_Service
    {
        [DI] private Enemy_Aspect aspect;
        [DI] private Destroy_Service destroyService;
        [DI] private LevelMap levelMap;
        [DI] private GOPool_Service goPoolService;
        [DI] private Enemy_Path_Service enemyPathService;
        [DI] private Movement_Service movementService;
        [DI] private Enemy_Converter converter;
        [DI] private State state;
        [DI] private EventBus events;
        
        private readonly GameObject enemiesContainer;

        public Enemy_Service()
        {
            enemiesContainer = GameObject.FindGameObjectWithTag(Constants.Tags.EnemiesContainer);
        }

        public bool HasEnemy(ProtoPackedEntity packedEntity, out int enemyEntity) => packedEntity.Unpack(aspect.World(), out enemyEntity) && aspect.enemyPool.Has(enemyEntity);
        public bool HasEnemy(ProtoPackedEntityWithWorld packedEntity, out int enemyEntity) => packedEntity.Unpack(out _, out enemyEntity) && aspect.enemyPool.Has(enemyEntity);
        public bool HasEnemy(int enemyEntity) => aspect.enemyPool.Has(enemyEntity);

        public ref Enemy GetEnemy(ProtoPackedEntity packedEntity)
        {
            if (!HasEnemy(packedEntity, out var enemyEntity)) throw new NullReferenceException("Entity don't have Enemy component. Use HasEnemy method before");
            return ref GetEnemy(enemyEntity);
        }
        public ref Enemy GetEnemy(ProtoPackedEntityWithWorld packedEntity)
        {
            if (!HasEnemy(packedEntity, out var enemyEntity)) throw new NullReferenceException("Entity don't have Enemy component. Use HasEnemy method before");
            return ref GetEnemy(enemyEntity);
        }
        public ref Enemy GetEnemy(int enemyEntity) => ref aspect.enemyPool.GetOrAdd(enemyEntity);

        public ref Ref<EnemyMonoBehaviour> GetEnemyMBRef(int enemyEntity) => ref aspect.enemyRefMBPool.GetOrAdd(enemyEntity);
        public EnemyMonoBehaviour GetEnemyMB(int enemyEntity) => GetEnemyMBRef(enemyEntity).reference!;

        public bool IsDead(int enemyEntity) => aspect.isEnemyDeadPool.Has(enemyEntity);
        public void SetIsDead(int enemyEntity, bool value) => aspect.isEnemyDeadPool.SetExistence(enemyEntity, value);
        
        public void Kill(int enemyEntity)
        {
            destroyService.SetIsDestroyed(enemyEntity, true);
            SetIsDead(enemyEntity, true);
        }
        
        public bool IsAlive(ProtoPackedEntity enemyPackedEntity, out int enemyEntity) =>
            enemyPackedEntity.Unpack(aspect.World(), out enemyEntity) && IsAlive(enemyEntity);

        public bool IsAlive(ProtoPackedEntityWithWorld enemyPackedEntity, out int enemyEntity) =>
            enemyPackedEntity.Unpack(out _, out enemyEntity) && IsAlive(enemyEntity);

        public bool IsAlive(int enemyEntity) =>
            HasEnemy(enemyEntity) &&
            !IsDead(enemyEntity) &&
            !destroyService.IsDestroyed(enemyEntity) &&
            !destroyService.IsDisabled(enemyEntity) &&
            movementService.HasGameObject(enemyEntity, true, true);

        private static float O(float value) => FloatUtils.DefaultIfZero(value, 1f);
        
        public int SpawnEnemy(
            string enemyName,
            int enemyType,
            int enemyVariant,
            SpawnData spawnData
        )
        {
            var enemyConfig = GetEnemyConfig(enemyName);
            EnemyConfigType? typedConfig = null;
            
            if (enemyConfig == null) return -1;

            var spawnCoords = levelMap.GetSpawn(spawnData.number);
            
            // enemyPathService.PrepareEnemyPath(ref spawnCoords, enemyEntity);
            var pathNumber = enemyPathService.RandomPathNumber(ref spawnCoords);
            var path = enemyPathService.GetPath(ref spawnCoords, pathNumber);
            
            // минимальный маршрут 3 клетки!!!
            if (path.Count < 3) return -1;

            var pathItem1 = path[0];
            var pathItem2 = path[1];
            var pathItem3 = path[2];

            ref var spawnCell = ref levelMap.GetCell(pathItem1.x, pathItem1.y);
            ref var nextCell = ref levelMap.GetCell(pathItem2.x, pathItem2.y);
            ref var nextNextCell = ref levelMap.GetCell(pathItem3.x, pathItem3.y);

            if (spawnCell.IsEmpty || nextCell.IsEmpty || nextNextCell.IsEmpty) return -1;
            
            // if (
            //     !levelMap.TryGetCell(pathItem1, out var spawnCell) ||
            //     !levelMap.TryGetCell(pathItem2, out var nextCell) || 
            //     !levelMap.TryGetCell(pathItem3, out var nextNextCell) 
            // ) return -1;

            var nextCoords = nextCell.coords;
            var nextNextCoords = nextNextCell.coords;
            
            var enemyPoolableObject = goPoolService.Get(
                enemyConfig.Value.prefab,
                enemiesContainer.transform,
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
            var enemyEntity = converter.GetEntity(enemyPoolableObject.gameObject) ?? aspect.World().NewEntity();
            converter.Convert(enemyPoolableObject.gameObject, enemyEntity);
            
            enemyPathService.SetPath(enemyEntity, ref spawnCoords, pathNumber);
            
            ref var enemy = ref GetEnemy(enemyEntity);
            enemy.enemyName = enemyName;
            enemy.spawmNumber = spawnData.number;
            enemy.health = O(spawnData.health);
            enemy.speed = O(spawnData.speed);
            enemy.damage = O(spawnData.damage);
            enemy.angularSpeed = 100f;

            ref var transform = ref movementService.GetTransform(enemyEntity);
            transform.ClearChangedStatus();
            
            // switch (enemyName.ToLower())
            // {
            //     case "creep":
            //     {
            //         var type = CreepEnemyMonoBehaviour.ParseType(enemyType);
            //
            //         if (enemyConfig.types.Length > 0)
            //         {
            //             typedConfig = enemyConfig.types[(int)type - 1];
            //             enemy.health *= typedConfig.baseHealth;
            //             enemy.speed *= typedConfig.baseSpeed;
            //             enemy.damage *= typedConfig.baseDamage;
            //             enemy.angularSpeed = typedConfig.angularSpeed;
            //         }
            //         break;
            //     }
            //     default:
            //         enemy.health *= enemyConfig.baseHealth;
            //         enemy.speed *= enemyConfig.baseHealth;
            //         enemy.damage *= enemyConfig.baseDamage;
            //         enemy.angularSpeed = enemyConfig.angularSpeed;
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

            transform.SetRotation(Enemy_Utils.LookToNextCell(ref spawnCell, ref nextCell));
            transform.SetPosition(Enemy_Utils.CalcPosition(spawnCell.coords, transform.rotation, enemy.offset));
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
                // todo ???? wtf typedConfig ????
                var animSpeed = typedConfig != null && typedConfig.Value.animationSpeed > 0
                    ? typedConfig.Value.animationSpeed
                    : enemyConfig.Value.animationSpeed;
                animSpeed = FloatUtils.DefaultIfZero(animSpeed, 1f);
                enemyMb.baseAnimationSpeed = animSpeed;
                enemyMb.animator!.speed = animSpeed * state.GetGameSpeed();
            }



            ref var movement = ref movementService.GetMovement(enemyEntity);
            movement.from = transform.position;
            movement.target = Enemy_Utils.CalcPosition(ref nextCoords, transform.rotation, enemy.offset);
            movement.nextTarget = Enemy_Utils.CalcPosition(ref nextNextCoords, Enemy_Utils.LookToNextCell(ref nextCell, ref nextNextCell), enemy.offset);
            movement.SetSpeed(enemy.speed, transform.rotation);
            movement.gapSqr = Constants.DefaultGapSqr;
            movement.speedOfGameAffected = true;
            
            SetIsDead(enemyEntity, false);

            // Debug.Log("New enemy:" + enemy);
            
            state.SetEnemiesCount(state.GetEnemiesCount() + 1);

            return enemyEntity;
        }

        private void ApplySpecificEnemyVariant(Enemy_Config enemyConfig, int enemyType, int enemyVariant, GameObject gameObject, ref Enemy enemy)
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
                aspect.World().DelEntity(entity);
            }
            Object.Destroy(o.gameObject);
        }

        public void ChangeHealthRelative(int enemyEntity, float helthRelative)
        {
            if (Mathf.Abs(helthRelative) < 0.0001f) return;
            GetEnemy(enemyEntity).health += helthRelative;
            events.global.Add<Event_Enemy_ChangeHealth>().Entity = aspect.World().PackEntityWithWorld(enemyEntity);
        }        
        
        public void ChangeHealth(int enemyEntity, float health)
        {
            ref var enemy = ref GetEnemy(enemyEntity);
            if (FloatUtils.IsEquals(enemy.health, health)) return;
            enemy.health = health;
            events.global.Add<Event_Enemy_ChangeHealth>().Entity = aspect.World().PackEntityWithWorld(enemyEntity);
        }

        public void ChangeSpeed(int enemyEntity, float speed)
        {
            ref var enemy = ref GetEnemy(enemyEntity);
            if (FloatUtils.IsEquals(enemy.speed, speed)) return;
            enemy.speed = speed;
            if (movementService.HasMovement(enemyEntity))
                movementService.GetMovement(enemyEntity).SetSpeed(enemy.speed);
        }

        public bool FindNearestEnemy(Vector2 position, float sqrRadius, out int nearestEnemyEntity)
        {
            var minSqrDistance = float.MaxValue;
            nearestEnemyEntity = -1;
            foreach (var enemyEntity in aspect.itLivingEnemies)
            {
                var enemyPosition = movementService.GetTransform(enemyEntity).position;
                
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

        public Slice<int> FindNearestEnemies(Vector2 position, float sqrMaxRadius, float sqrMinRadius = 0f)
        {
            var nearestEnemies = new Slice<int>(10);
            foreach (var enemyEntity in aspect.itLivingEnemies)
            {
                var enemyPosition = movementService.GetTransform(enemyEntity).position;
                
                var dx = enemyPosition.x - position.x;
                var dy = enemyPosition.y - position.y;
                var sqrDx = dx * dx;
                var sqrDy = dy * dy;

                if (sqrDx > sqrMaxRadius || sqrDy > sqrMaxRadius) continue;

                var toEnemy = enemyPosition - position;
                var sqrDistanse = toEnemy.x * toEnemy.x + toEnemy.y * toEnemy.y;
                
                if (sqrDistanse > sqrMaxRadius || sqrDistanse < sqrMinRadius) continue;
                
                nearestEnemies.Add(enemyEntity);
            }

            return nearestEnemies;
        }
        
        public Enemy_Config? GetEnemyConfig(string enemyName)
        {
            var enemyNameLowerCase = enemyName.ToLower();
            foreach (var enemyConfig in ServiceContainer.Get<Enemy_Config[]>())
            {
                if (enemyConfig.name == enemyName || enemyConfig.name == enemyNameLowerCase)
                {
                    return enemyConfig;
                }
            }

            return null;
        }

        public ProtoWorld World() => aspect.World();
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