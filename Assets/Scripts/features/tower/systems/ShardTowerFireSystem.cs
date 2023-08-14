using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy;
using td.features.level;
using td.features.movement;
using td.features.projectile;
using td.features.shard;
using td.features.state;
using td.utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace td.features.tower.systems
{
    public class ShardTowerFireSystem : IProtoRunSystem
    {
        [DI] private Tower_Aspect aspect;
        [DI] private State state;
        [DI] private Projectile_Service projectileService;
        [DI] private Shard_Calculator calc;
        [DI] private Movement_Service movementService;
        [DI] private Shard_Service shardService;
        [DI] private Enemy_Service enemyService;
        [DI] private ShardsConfig shardsConfig; // todo
        [DI] private LevelMap levelMap;
        
        public void Run()
        {
            foreach (var towerEntity in aspect.itShardTower)
            {
                ref var tower = ref aspect.towerPool.Get(towerEntity);
                ref var shardTower = ref aspect.shardTowerPool.Get(towerEntity);
                ref var targetPackedEntity = ref aspect.towerTargetPool.Get(towerEntity).targetEntity;
                var shardPackedEntity = aspect.shardTowerWithShardPool.Get(towerEntity).shardEntity;
                
                if (!shardPackedEntity.Unpack(out _, out var shardEntity) ||
                    !targetPackedEntity.Unpack(out _, out var targetEnemyEntity)) continue;
                    
                ref var shard = ref shardService.GetShard(shardEntity);
                
                var towerTransform = movementService.GetGOTransform(towerEntity);
                
                if (shardTower.fireCountdown > 0) shardTower.fireCountdown -= Time.deltaTime * state.GetGameSpeed();
                if (shardTower.fireCountdown > Constants.ZeroFloat) continue;

                var targetEnemyTransform = movementService.GetTransform(targetEnemyEntity);

                var projectilePosition = (Vector2)towerTransform.position + tower.barrel;
                var projectileTarget = (Vector2)targetEnemyTransform.position;
                
                // Рассчитываем вектор от башни к врагу
                var toTarget = projectileTarget - projectilePosition;
                
                var sqrDistance = (toTarget).sqrMagnitude;

                // yellow - увеличивает радиус стрельбы
                var radius = calc.GetTowerRadius(ref shard);

                if (sqrDistance > radius * radius) continue;
                
                // --------------------------------

                var speed = calc.GetProjectileSpeed(ref shard);
                
                var enemyScale = movementService.GetTransform(targetEnemyEntity).scale;
                var scale = Mathf.Max(enemyScale.x, enemyScale.y);
                
                if (movementService.HasMovement(targetEnemyEntity) && !movementService.IsFreezed(targetEnemyEntity))
                {
                    ref var targetMovement = ref movementService.GetMovement(targetEnemyEntity);
                    
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
                
                calc.CalculateSpread(ref shard, out var maxSpread, out var distanceFactor);
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
                var fireRate = calc.GetFireRate(ref shard);

                var projectileEntity = projectileService.SpawnProjectile(
                    "bullet",
                    projectilePosition,
                    projectileTarget,
                    speed,
                    sqrGap,
                    towerEntity,
                    ShardUtils.GetMixedColor(ref shard, shardsConfig)
                );

                // all
                if (calc.HasBaseDamage(ref shard))
                {
                    ref var damageAtr = ref projectileService.GetDamageAttribute(projectileEntity);
                    calc.CalculateBaseDamageParams(
                        shard: ref shard,
                        damage: out damageAtr.damage,
                        type: out damageAtr.type
                    );
                }

                // red - разрывной. удар по области
                if (calc.HasExplosive(ref shard))
                {
                    ref var explosiveAtr = ref projectileService.GetExplosiveAttribute(projectileEntity);
                    calc.CalculateExplosiveParams(
                        shard: ref shard,
                        damage: out explosiveAtr.damage,
                        diameter: out explosiveAtr.diameter,
                        damageFading: out explosiveAtr.damageFading
                    );
                }

                // green - отравляет мобов на время
                if (calc.HasPoison(ref shard))
                {
                    ref var poisonAtr = ref projectileService.GetPoisonAttribute(projectileEntity);
                    calc.CalculatePoisonParams(
                        shard: ref shard,
                        damage: out poisonAtr.damage,
                        duration: out poisonAtr.duration
                    );
                }
                
                // blue - замедляет мобов на время
                if (calc.HasSlowing(ref shard))
                {
                    ref var slowingAtr = ref projectileService.GetSlowingAttribute(projectileEntity);
                    calc.CalculateSlowingParams(
                        shard: ref shard,
                        power: out slowingAtr.speedMultipler,
                        duration: out slowingAtr.duration
                    );
                }

                // aquamarine - молния. цепная реакция от моба к мобу
                if (calc.HasLightning(ref shard))
                {
                    ref var lightningAtr = ref projectileService.GetLightningAttribute(projectileEntity);
                    calc.CalculateLightningParams(
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
                if (calc.HasShocking(ref shard))
                {
                    ref var shockingAtr = ref projectileService.GetShockingAttribute(projectileEntity);
                    calc.CalculateShockingParams(
                        shard: ref shard,
                        duration: out shockingAtr.duration,
                        probability: out shockingAtr.probability
                    );
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
    }
}