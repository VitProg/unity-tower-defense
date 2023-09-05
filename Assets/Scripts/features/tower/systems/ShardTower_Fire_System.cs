using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features._common;
using td.features.building;
using td.features.enemy;
using td.features.level;
using td.features.level.cells;
using td.features.movement;
using td.features.projectile;
using td.features.shard;
using td.features.shard.data;
using td.features.state;
using td.utils;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace td.features.tower.systems
{
    public class ShardTower_Fire_System : IProtoRunSystem
    {
        [DI] private Tower_Aspect aspect;
        [DI] private State state;
        [DI] private Projectile_Service projectileService;
        [DI] private Shard_Calculator calc;
        [DI] private Common_Service common;
        [DI] private Movement_Service movementService;
        [DI] private Shard_Service shardService;
        [DI] private Enemy_Service enemyService;
        [DI] private Shards_Config_SO shardsConfigSO; // todo
        [DI] private Level_State levelState;
        [DI] private Building_Service buildingService;
        
        //
        private float2 projectilePosition;
        private float2 projectileTarget;
        private float2 toTarget;
        private Vector2 spreadVector;
        //
        
        public void Run()
        {
            foreach (var towerEntity in aspect.itShardTowerWithTarget)
            {
                ref var building = ref buildingService.GetBuilding(towerEntity);

                if (!levelState.HasCell(building.coords, CellTypes.CanBuild)) continue;
                
                ref var shardTower = ref aspect.shardTowerPool.Get(towerEntity);
                ref var cell = ref levelState.GetCell(building.coords, CellTypes.CanBuild);

                if (!cell.HasBuilding() || !cell.HasShard()) continue;
                if (!shardService.HasShard(cell.packedShardEntity, out var shardEntity)) continue;

                // timer
                if (shardTower.fireCountdown > 0) shardTower.fireCountdown -= Time.deltaTime * state.GetGameSpeed();
                if (shardTower.fireCountdown > Constants.ZeroFloat) continue;
                //
                
                ref var shard = ref shardService.GetShard(shardEntity);
                
                ref var enemyPackedEntity = ref aspect.towerTargetPool.Get(towerEntity).targetEntity;
                ref var enemy = ref enemyService.GetEnemy(enemyPackedEntity, out var enemyEntity);
                ref var enemyTransform = ref movementService.GetTransform(enemyEntity);
                
                ref var shardTowerTransform = ref movementService.GetTransform(towerEntity);
                ref var shardTowerTargetPoint = ref movementService.GetTargetPointPool(towerEntity);

                projectilePosition.x = shardTowerTransform.position.x + shardTowerTargetPoint.Point.x;
                projectilePosition.y = shardTowerTransform.position.y + shardTowerTargetPoint.Point.y;
                
                // Рассчитываем вектор от башни к врагу
                toTarget.x = enemyTransform.position.x - projectilePosition.x;
                toTarget.y = enemyTransform.position.y - projectilePosition.y;
                
                var sqrDistance = toTarget.SqrMagnitude();
                
                if (sqrDistance > shardTower.sqrRadius) continue;
                
                // --------------------------------

                var speed = calc.GetProjectileSpeed(ref shard);
                var scale = MathFast.Max(enemyTransform.scale.x, enemyTransform.scale.y);

                projectileTarget.x = enemyTransform.position.x;
                projectileTarget.y = enemyTransform.position.y;
                
                if (movementService.HasMovement(enemyEntity) && !movementService.IsFreezed(enemyEntity))
                {
                    ref var targetMovement = ref movementService.GetMovement(enemyEntity);
                    
                    // Рассчитываем привентивную стрельбу с учетом вектора скорости врага, скорости снаряда и расстояния до цели
                    if (!targetMovement.speedV.IsZero())
                    {
                        var (targetFuturePosition, sqrDist) = CalculatePredictedEnemyPosition(
                            enemyTransform.position,
                            targetMovement.speedV,
                            projectilePosition,
                            speed
                        );
                        // Debug.Log(new {targetFuturePosition, projectileTarget, distance, d});

                        if (sqrDist > shardTower.sqrRadius) continue;

                        projectileTarget.x = targetFuturePosition.x;
                        projectileTarget.y = targetFuturePosition.y;
                    } 
                }
                
                calc.CalculateSpread(ref shard, out var maxSpread, out var distanceFactor);
                var spreadFactor = Random.Range(0f, maxSpread) * (sqrDistance * distanceFactor);
                if (spreadFactor > 0.001f)
                {
                    spreadVector = Random.insideUnitCircle * spreadFactor;
                    projectileTarget.x += spreadVector.x;
                    projectileTarget.y += spreadVector.y;
                }

                var projectileEntity = projectileService.SpawnProjectile(
                    Constants.Prjoctiles.Bullet,
                    projectilePosition,
                    projectileTarget,
                    speed,
                    (.2f * scale) * (.2f * scale),
                    towerEntity,
                    shard.currentColor
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

                shardTower.fireCountdown = shard.fireCountdown;
            }
        }
        
        private (float2 position, float sqrDistanse) CalculatePredictedEnemyPosition(
            float2 enemyPosition,
            float2 enemySpeed,
            float2 towerPosition,
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
                var distanseToPredictedEnemyPosition = toPredictedEnemyPosition.Magnitude();

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
            return (enemyPosition, (enemyPosition - towerPosition).SqrMagnitude());
        }
    }
}