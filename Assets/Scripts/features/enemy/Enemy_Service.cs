using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features._common;
using td.features._common.components;
using td.features.destroy;
using td.features.enemy.bus;
using td.features.enemy.components;
using td.features.enemy.data;
using td.features.enemy.enemyPath;
using td.features.enemy.mb;
using td.features.eventBus;
using td.features.goPool;
using td.features.level;
using td.features.movement;
using td.features.state;
using td.utils;
using td.utils.di;
using td.utils.ecs;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.enemy {
    public class Enemy_Service {
        [DI] private Enemy_Aspect aspect;
        [DI] private Destroy_Service destroyService;
        [DI] private Level_State levelState;
        [DI] private GOPool_Service goPoolService;
        [DI] private EnemyPath_Service enemyPathService;
        [DI] private EnemyPath_State enemyPathState;
        [DI] private Movement_Service movementService;
        [DI] private Common_Service common;
        [DI] private Enemy_Converter converter;
        [DI] private State state;
        [DI] private EventBus events;

        private readonly GameObject enemiesContainer;

        public Enemy_Service() {
            enemiesContainer = GameObject.FindGameObjectWithTag(Constants.Tags.EnemiesContainer);
        }

        public bool HasEnemy(ProtoPackedEntity packedEntity, out int enemyEntity) =>
            packedEntity.Unpack(aspect.World(), out enemyEntity) && aspect.enemyPool.Has(enemyEntity);

        public bool HasEnemy(ProtoPackedEntityWithWorld packedEntity, out int enemyEntity) =>
            packedEntity.Unpack(out _, out enemyEntity) && aspect.enemyPool.Has(enemyEntity);

        public bool HasEnemy(int enemyEntity) => aspect.enemyPool.Has(enemyEntity);

        public ref Enemy GetEnemy(ProtoPackedEntityWithWorld packedEntity, out int enemyEntity) {
            var check = HasEnemy(packedEntity, out enemyEntity);
#if UNITY_EDITOR
            if (!check) throw new NullReferenceException("Entity don't have Enemy component. Use HasEnemy method before");
#endif
            return ref GetEnemy(enemyEntity);
        }

        public ref Enemy GetEnemy(int enemyEntity) => ref aspect.enemyPool.GetOrAdd(enemyEntity);

        public ref Ref<EnemyMonoBehaviour> GetEnemyMBRef(int enemyEntity) => ref aspect.enemyRefMBPool.GetOrAdd(enemyEntity);
        public EnemyMonoBehaviour GetEnemyMB(int enemyEntity) => GetEnemyMBRef(enemyEntity).reference!;

        public bool IsDead(int enemyEntity) => aspect.isEnemyDeadPool.Has(enemyEntity);
        public void SetIsDead(int enemyEntity, bool value) => aspect.isEnemyDeadPool.SetExistence(enemyEntity, value);

        public void Kill(int enemyEntity) {
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
            common.HasGameObject(enemyEntity, true, true);

        public int SpawnEnemy(
            string enemyName,
            int enemyType,
            int enemyVariant,
            // Spawn Data
            int spawnPointNumber,
            float health,
            float speed,
            float damage,
            float scaleMin,
            float scaleMax,
            float offsetMin,
            float offsetMax
        ) {
            var enemyConfig = GetEnemyConfig(enemyName);
            EnemyConfigType? typedConfig = null;

            if (enemyConfig == null) return -1;

            ref var spawnCell = ref levelState.GetSpawn(spawnPointNumber);

            var routeIdx = enemyPathService.RandomPathNumber(ref spawnCell.coords);
            var routeLength = enemyPathState.GetRouteLength(routeIdx);

            // минимальный маршрут 3 клетки!!!
            if (routeLength < 3) return -1;

            ref var pathItem1 = ref enemyPathState.GetRouteItem(routeIdx, 0);
            ref var pathItem2 = ref enemyPathState.GetRouteItem(routeIdx, 1);
            ref var pathItem3 = ref enemyPathState.GetRouteItem(routeIdx, 2);

            ref var nextCell = ref levelState.GetCell(pathItem2.x, pathItem2.y);
            ref var nextNextCell = ref levelState.GetCell(pathItem3.x, pathItem3.y);

            if (spawnCell.IsEmpty || nextCell.IsEmpty || nextNextCell.IsEmpty) return -1;

            var nextCoords = nextCell.coords;
            var nextNextCoords = nextNextCell.coords;

            var enemyPoolableObject = goPoolService.Get(
                enemyConfig.Value.prefab,
                enemiesContainer.transform,
                Constants.Pools.EnemyDefaultCopacity,
                Constants.Pools.EnemyMaxCopacity,
                null,
                null,
                delegate(PoolableObject go) {
                    go.gameObject.SetActive(false);
                    if (!go.transform.TryGetComponent(out SpriteRenderer sr)) {
                        sr = go.transform.GetComponentInChildren<SpriteRenderer>();
                    }

                    if (sr != null) sr.color = Color.white;
                },
                ActionOnDestroy
            );
            var enemyEntity = converter.GetEntity(enemyPoolableObject.gameObject) ?? aspect.World().NewEntity();
            converter.Convert(enemyPoolableObject.gameObject, enemyEntity);

            enemyPathService.SetRoute(enemyEntity, routeIdx);

            ref var enemy = ref GetEnemy(enemyEntity);
            enemy.enemyName = enemyName;
            enemy.spawnPoinsNumber = spawnPointNumber;
            enemy.health = FloatUtils.DefaultIfZero(health);
            enemy.speed = FloatUtils.DefaultIfZero(speed);
            enemy.damage = FloatUtils.DefaultIfZero(damage);
            enemy.angularSpeed = 100f;

            ref var transform = ref movementService.GetTransform(enemyEntity);
            transform.ClearChangedStatus();

            ApplySpecificEnemyVariant(
                enemyConfig.Value,
                enemyType,
                enemyVariant,
                enemyPoolableObject.gameObject,
                ref enemy
            );
            enemy.angularSpeed *= MathFast.Clamp(speed / 5f, 1f, 100f);

            transform.SetScale(
                RandomUtils.Range(
                    scaleMin > 0.001f ? scaleMin : Constants.Enemy.MinSize,
                    scaleMax > 0.001f ? scaleMax : Constants.Enemy.MaxSize
                )
            );
            enemy.offset.x = RandomUtils.Range(
                offsetMin > 0.001f ? offsetMin : Constants.Enemy.OffsetMin,
                offsetMax > 0.001f ? offsetMax : Constants.Enemy.OffsetMax
            );
            enemy.offset.y = RandomUtils.Range(
                offsetMin > 0.001f ? offsetMin : Constants.Enemy.OffsetMin,
                offsetMax > 0.001f ? offsetMax : Constants.Enemy.OffsetMax
            );
            enemy.energy = (uint)MathFast.Max(
                (enemy.health * enemy.damage * enemy.speed * transform.ScaleScalar) / 5,
                1
            ); //todo move to utils/helper/service/calculator...

            enemy.offset.x = MathFast.Clamp(enemy.offset.x, Constants.Enemy.OffsetMin, Constants.Enemy.OffsetMax);
            enemy.offset.y = MathFast.Clamp(enemy.offset.y, Constants.Enemy.OffsetMin, Constants.Enemy.OffsetMax);
            // enemy.offset.x = 0;
            // enemy.offset.y = 0;

            enemy.startingHealth = enemy.health;
            enemy.startingSpeed = enemy.speed;

            transform.SetRotation(Enemy_Utils.LookToNextCell(ref spawnCell, ref nextCell));
            transform.SetPosition(Enemy_Utils.CalcPosition(spawnCell.coords, transform.rotation, enemy.offset));
            var goTransform = enemyPoolableObject.transform;
            goTransform.position = transform.position.ToVector3();
            goTransform.localScale = transform.GetScaleVector();

            var enemyMb = GetEnemyMB(enemyEntity); //enemyPoolableObject.GetComponent<EnemyMonoBehaviour>());
            enemyMb.body.transform.rotation = transform.rotation;
            enemyMb.hp.minValue = 0.0f;
            enemyMb.hp.maxValue = enemy.health;
            enemyMb.hp.value = enemy.health;
            enemyMb.hp.gameObject.SetActive(false);
            enemyMb.hpLine.color = Constants.Enemy.HpBarColors[^1];

            if (enemyMb.animator != null) {
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

            // state.SetEnemiesCount(state.GetEnemiesCount() + 1);

            events.global.Add<Event_Enemy_Spawned>().Entity = World().PackEntityWithWorld(enemyEntity);

            return enemyEntity;
        }

        private void ApplySpecificEnemyVariant(Enemy_Config enemyConfig, int enemyType, int enemyVariant, GameObject gameObject, ref Enemy enemy) {
            switch (enemyConfig.name.ToLower()) {
                case "creep": {
                    var creepType = CreepEnemyMonoBehaviour.ParseType(enemyType);
                    var creepVariant = CreepEnemyMonoBehaviour.ParseVariant(enemyVariant);

                    if (enemyConfig.types.Length > 0) {
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

        private void ActionOnDestroy(PoolableObject o) {
            var ecsEntity = o.GetComponent<EcsEntity>();
            if (ecsEntity.packedEntity.Unpack(out _, out var entity)) {
                aspect.World().DelEntity(entity);
            }

            Object.Destroy(o.gameObject);
        }

        public void ChangeHealthRelative(int enemyEntity, float helthRelative) {
            if (MathFast.Abs(helthRelative) < 0.0001f) return;
            GetEnemy(enemyEntity).health += helthRelative;
            events.global.Add<Event_Enemy_ChangeHealth>().Entity = aspect.World().PackEntityWithWorld(enemyEntity);
        }

        public void ChangeHealth(int enemyEntity, float health) {
            ref var enemy = ref GetEnemy(enemyEntity);
            if (FloatUtils.IsEquals(enemy.health, health)) return;
            enemy.health = health;
            events.global.Add<Event_Enemy_ChangeHealth>().Entity = aspect.World().PackEntityWithWorld(enemyEntity);
        }

        public void ChangeSpeed(int enemyEntity, float speed) {
            ref var enemy = ref GetEnemy(enemyEntity);
            if (FloatUtils.IsEquals(enemy.speed, speed)) return;
            enemy.speed = speed;
            if (movementService.HasMovement(enemyEntity)) movementService.GetMovement(enemyEntity).SetSpeed(enemy.speed);
        }

        public bool FindNearestEnemy(float2 position, float sqrRadius, out int nearestEnemyEntity) {
            var minSqrDistance = float.MaxValue;
            nearestEnemyEntity = -1;

            // todo use chached iterator
            foreach (var enemyEntity in aspect.itLivingEnemies) {
                var enemyPosition = movementService.GetTransform(enemyEntity).position;

                var dx = enemyPosition.x - position.x;
                var dy = enemyPosition.y - position.y;
                var sqrDx = dx * dx;
                var sqrDy = dy * dy;

                if (sqrDx > sqrRadius || sqrDy > sqrRadius) continue;

                var toEnemy = enemyPosition - position;
                var sqrDistanse = toEnemy.SqrMagnitude();

                if (sqrDistanse > sqrRadius) continue;
                if (minSqrDistance < sqrDistanse) continue;

                minSqrDistance = sqrDistanse;
                nearestEnemyEntity = enemyEntity;
            }

            Debug.Log("> FindNearestEnemy result " + nearestEnemyEntity);
            
            return nearestEnemyEntity > -1;
        }

        public Slice<int> FindNearestEnemies(float2 position, float sqrMaxRadius, float sqrMinRadius = 0f) {
            var nearestEnemies = new Slice<int>(10);
            
            // todo use chached iterator
            foreach (var enemyEntity in aspect.itLivingEnemies) {
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

        private Enemy_Config[] enemyConfigs;

        public Enemy_Config? GetEnemyConfig(string enemyName) {
            var enemyNameLowerCase = enemyName.ToLower();

            if (enemyConfigs == null) {
                enemyConfigs = ServiceContainer.Get<Enemy_Config[]>();
            }

            foreach (var enemyConfig in enemyConfigs) {
                if (enemyConfig.name == enemyName || enemyConfig.name == enemyNameLowerCase) {
                    return enemyConfig;
                }
            }

            return null;
        }

        public ProtoWorld World() => aspect.World();
    }

    public struct SpawnData {
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
