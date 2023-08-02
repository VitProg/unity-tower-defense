using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.enemy;
using td.features.level;
using td.features.projectile;
using td.features.shard;
using td.features.shard.flags;
using td.features.state;
using td.features.tower.components;
using td.utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace td.features.tower.systems
{
    public class ShardTowerFireSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Projectile_Service> projectileService;
        private readonly EcsInject<ShardCalculator> calc;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<Shard_Service> shardService;
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<ShardsConfig> shardsConfig; // todo
        private readonly EcsInject<LevelMap> levelMap;
        
        private readonly EcsWorldInject world;

        private readonly EcsFilterInject<
            Inc<Tower, ShardTower, Ref<GameObject>, TowerTarget, ShardTowerWithShard/*??*/>,
            ExcludeNotAlive
        > towerEntities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var towerEntity in towerEntities.Value)
            {
                ref var tower = ref towerEntities.Pools.Inc1.Get(towerEntity);
                ref var shardTower = ref towerEntities.Pools.Inc2.Get(towerEntity);
                ref var targetPackedEntity = ref towerEntities.Pools.Inc4.Get(towerEntity).targetEntity;
                var shardPackedEntity = towerEntities.Pools.Inc5.Get(towerEntity).shardEntity;
                
                if (!shardPackedEntity.Unpack(out _, out var shardEntity) ||
                    !targetPackedEntity.Unpack(world.Value, out var targetEnemyEntity)) continue;
                    
                ref var shard = ref shardService.Value.GetShard(shardEntity);
                
                var towerTransform = common.Value.GetGOTransform(towerEntity);
                
                if (shardTower.fireCountdown > 0) shardTower.fireCountdown -= Time.deltaTime * state.Value.GameSpeed;
                if (shardTower.fireCountdown > Constants.ZeroFloat) continue;

                var targetEnemyTransform = common.Value.GetTransform(targetEnemyEntity)!;

                var projectilePosition = (Vector2)towerTransform.position + tower.barrel;
                var projectileTarget = (Vector2)targetEnemyTransform.position;
                
                // Рассчитываем вектор от башни к врагу
                var toTarget = projectileTarget - projectilePosition;
                
                var sqrDistance = (toTarget).sqrMagnitude;

                // yellow - увеличивает радиус стрельбы
                var radius = calc.Value.GetTowerRadius(ref shard);

                if (sqrDistance > radius * radius) continue;
                
                // --------------------------------

                var speed = calc.Value.GetProjectileSpeed(ref shard);
                
                var enemyScale = common.Value.GetTransform(targetEnemyEntity).scale;
                var scale = Mathf.Max(enemyScale.x, enemyScale.y);
                
                if (common.Value.HasMovement(targetEnemyEntity) && !common.Value.IsFreezed(targetEnemyEntity))
                {
                    ref var targetMovement = ref common.Value.GetMovement(targetEnemyEntity);
                    
                    // Рассчитываем привентивную стрельбу с учетом вектора скорости врага, скорости снаряда и расстояния до цели
                    if (!targetMovement.speedV.IsZero())
                    {
                        var (targetFuturePosition, distance) = CalculatePredictedEnemyPosition(
                            targetEnemyTransform.position,
                            targetMovement.speedV,
                            projectilePosition,
                            speed
                        );
                        // Debug.Log(new {targetFuturePosition, projectileTarget, distance, d});

                        if (distance > radius) continue;

                        projectileTarget = targetFuturePosition;
                    } 
                }
                
                calc.Value.CalculateSpread(ref shard, out var maxSpread, out var distanceFactor);
                var spread = Random.Range(0f, maxSpread);
                var spreadFactor = spread * (sqrDistance * distanceFactor);
                if (spreadFactor > 0.001f)
                {
                    var spreadVector = Random.insideUnitCircle * spreadFactor;
                    projectileTarget += spreadVector;
                    // Debug.Log(new {maxSpread, distanceFactor, spread, spreadFactor, spreadVector, sqrDistance});
                }

                var sqrGap = .2f * scale;
                sqrGap *= sqrGap;

                // pink - увеличивает скорострельность
                var fireRate = calc.Value.GetFireRate(ref shard);

                var projectileEntity = projectileService.Value.SpawnProjectile(
                    "bullet",
                    projectilePosition,
                    projectileTarget,
                    speed,
                    sqrGap,
                    towerEntity,
                    ShardUtils.GetMixedColor(ref shard, shardsConfig)
                );

                // all
                if (calc.Value.HasBaseDamage(ref shard))
                {
                    ref var damageAtr = ref projectileService.Value.GetDamageAttribute(projectileEntity);
                    calc.Value.CalculateBaseDamageParams(ref shard, out damageAtr.damage);
                }

                // red - разрывной. удар по области
                if (calc.Value.HasExplosive(ref shard))
                {
                    ref var explosiveAtr = ref projectileService.Value.GetExplosiveAttribute(projectileEntity);
                    calc.Value.CalculateExplosiveParams(ref shard, out explosiveAtr.damage, out explosiveAtr.diameter, out explosiveAtr.damageFading);
                }

                // green - отравляет мобов на время
                if (calc.Value.HasPoison(ref shard))
                {
                    ref var poisonAtr = ref projectileService.Value.GetPoisonAttribute(projectileEntity);
                    calc.Value.CalculatePoisonParams(ref shard, out poisonAtr.damageInterval, out poisonAtr.interval, out poisonAtr.duration);
                }
                
                // blue - замедляет мобов на время
                if (calc.Value.HasSlowing(ref shard))
                {
                    ref var slowingAtr = ref projectileService.Value.GetSlowingAttribute(projectileEntity);
                    calc.Value.CalculateSlowingParams(ref shard, out slowingAtr.speedMultipler, out slowingAtr.duration);
                }

                // aquamarine - молния. цепная реакция от моба к мобу
                if (calc.Value.HasLightning(ref shard))
                {
                    ref var lightningAtr = ref projectileService.Value.GetLightningAttribute(projectileEntity);
                    calc.Value.CalculateLightningParams(
                        ref shard, 
                        out lightningAtr.duration,
                        out lightningAtr.damage,
                        out lightningAtr.damageReduction,
                        out lightningAtr.damageInterval,
                        out lightningAtr.chainReaction,
                        out lightningAtr.chainReactionRadius
                    );
                }

                // violet - шок, кантузия… останавливает цель на короткое время. срабатывает с % вероятности
                if (calc.Value.HasShocking(ref shard))
                {
                    ref var shockingAtr = ref projectileService.Value.GetShockingAttribute(projectileEntity);
                    calc.Value.CalculateShockingParams(ref shard, out shockingAtr.duration, out shockingAtr.probability);
                }
                
                // ToDo orange - увеличивает приток энергии от убитых им мобов

                shardTower.fireCountdown = 1f / fireRate;
            }
        }
        
        private (Vector2 position, float distanse) CalculatePredictedEnemyPosition(
            Vector2 enemyPosition,
            Vector2 enemySpeed,
            Vector2 towerPosition,
            float projectileSpeed
        )
        {
            const float timeStep = 0.033333f; // Время шага для поиска
            const float timeStepDraft = 0.1f; // Время шага для примерного поиска
            const int maxSteps = 100;

            var isDraft = true;
            var iteration = 0;

            var t = 0f;

            for (var i = 1; i < maxSteps; i++)
            {
                // Рассчитываем предполагаемую позицию врага через время t
                var predictedPosition = enemyPosition + enemySpeed * t;

                // Рассчитываем вектор от башни к предполагаемой позиции врага
                var toPredictedEnemyPosition = predictedPosition - towerPosition;

                // Расстояние от башни до предпологаемой позиции врага
                var distanseToPredictedEnemyPosition = toPredictedEnemyPosition.magnitude;

                // С какой скоростью надо лететь снаряду, чтобы достичь тоже точки в тоже время
                var speedNeaded = distanseToPredictedEnemyPosition / t;

                // Если эта скорость меньшей текущей скорости снаряда, то мы либо уже нашли точку, либо переходим к более точному поиску
                if (speedNeaded < projectileSpeed)
                {
                    if (isDraft)
                    {
                        t -= timeStepDraft;
                        // Debug.Log("DRAFT iteration = " + iteration + ", t = " + t);
                        isDraft = false;
                    }
                    else
                    {
                        if (iteration > 40)
                        {
                            Debug.LogWarning("iteration = " + iteration + ", t = " + t);
                        }

                        return (predictedPosition, distanseToPredictedEnemyPosition);
                    }
                }

                t += isDraft ? timeStepDraft : timeStep;
                iteration++;
            }
            
            // Если не смогли рассчитать то просто стреляем до текущего положения врага
            return (enemyPosition, (enemyPosition - towerPosition).magnitude);
        }

        public void Init(IEcsSystems systems)
        {
            
        }
    }
}