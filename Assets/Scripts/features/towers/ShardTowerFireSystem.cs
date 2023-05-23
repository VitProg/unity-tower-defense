using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.components.refs;
using td.features.dragNDrop;
using td.features.projectiles;
using td.features.projectiles.attributes;
using td.features.shards;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.towers
{
    public class ShardTowerFireSystem : IEcsRunSystem, IEcsInitSystem
    {
        [Inject] private LevelMap levelMap;
        [Inject] private ProjectileService projectileService;
        [Inject] private ShardCalculator shardCalculator;
        [InjectWorld] private EcsWorld world;

        private readonly EcsFilterInject<
            Inc<ShardTower, Tower, Ref<GameObject>>,
            Exc<IsDragging, IsDisabled, IsDestroyed>
        > towerEntities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var cannonEntity in towerEntities.Value)
            {
                ref var shardTower = ref towerEntities.Pools.Inc1.Get(cannonEntity);
                ref var tower = ref towerEntities.Pools.Inc2.Get(cannonEntity);
                var towerGORef = towerEntities.Pools.Inc3.Get(cannonEntity);
                
                var lunchProjectile = false;

                if (shardTower.fireCountdown > 0) shardTower.fireCountdown -= Time.deltaTime;
                if (shardTower.fireCountdown < Constants.ZeroFloat) lunchProjectile = true;
                if (!lunchProjectile || !world.HasComponent<ProjectileTarget>(cannonEntity)) continue;

                var target = world.GetComponent<ProjectileTarget>(cannonEntity);
                if (!target.TargetEntity.Unpack(world, out var enemyEntity)) continue;
                
                if (!shardTower.shard.Unpack(world, out var shardEntity)) continue;
                ref var shard = ref world.GetComponent<Shard>(shardEntity);
                
                ref var enemyGORef = ref world.GetComponent<Ref<GameObject>>(enemyEntity);
                    
                var enemyPostiion = (Vector2)enemyGORef.reference.transform.position;
                var projectilePosition = (Vector2)towerGORef.reference.transform.position + tower.barrel;
                var projectileTarget = enemyPostiion;

                var sqrDistance = (projectilePosition - projectileTarget).sqrMagnitude;

                // yellow - увеличивает радиус стрельбы
                var radius = shardCalculator.GetTowerRadius(ref shard);
                
                var sqrRadius = radius * radius;

                if (sqrDistance > sqrRadius) continue;
                
                var speed = shardCalculator.GetProjectileSpeed(ref shard);
                
                // pink - увеличивает скорострельность
                var fireRate = shardCalculator.GetFireRate(ref shard);

                var projectileEntity = projectileService.SpawnProjectile(
                    name: "bullet",
                    position: projectilePosition,
                    targetEntity: enemyEntity,
                    speed: speed,
                    whoFired: cannonEntity,
                    ref shard // todo
                );

                // all
                if (shardCalculator.HasBaseDamage(ref shard))
                {
                    ref var damageAtr = ref world.GetComponent<DamageAttribute>(projectileEntity);
                    shardCalculator.CalculateBaseDamageParams(ref shard, out damageAtr.damage);
                }

                // red - разрывной. удар по области
                if (shardCalculator.HasExplosive(ref shard))
                {
                    ref var explosiveAtr = ref world.GetComponent<ExplosiveAttribute>(projectileEntity);
                    shardCalculator.CalculateExplosiveParams(ref shard, out explosiveAtr.damage, out explosiveAtr.diameter, out explosiveAtr.damageFading);
                }

                // green - отравляет мобов на время
                if (shardCalculator.HasPoison(ref shard))
                {
                    ref var poisonAtr = ref world.GetComponent<PoisonAttribute>(projectileEntity);
                    shardCalculator.CalculatePoisonParams(ref shard, out poisonAtr.damageInterval, out poisonAtr.interval, out poisonAtr.duration);
                }
                
                // blue - замедляет мобов на время
                if (shardCalculator.HasSlowing(ref shard))
                {
                    ref var slowingAtr = ref world.GetComponent<SlowingAttribute>(projectileEntity);
                    shardCalculator.CalculateSlowingParams(ref shard, out slowingAtr.speedMultipler, out slowingAtr.duration);
                }

                // aquamarine - молния. цепная реакция от моба к мобу
                if (shardCalculator.HasLightning(ref shard))
                {
                    ref var lightningAtr = ref world.GetComponent<LightningAttribute>(projectileEntity);
                    shardCalculator.CalculateLightningParams(ref shard, out lightningAtr.damage, out lightningAtr.damageReduction, out lightningAtr.chainReaction);
                }

                // violet - шок, кантузия… останавливает цель на короткое время. срабатывает с % вероятности
                if (shardCalculator.HasShocking(ref shard))
                {
                    ref var shockingAtr = ref world.GetComponent<ShockingAttribute>(projectileEntity);
                    shardCalculator.CalculateShockingParams(ref shard, out shockingAtr.duration, out shockingAtr.probability);
                }
                
                // ToDo orange - увеличивает приток энергии от убитых им мобов

                shardTower.fireCountdown = 1f / fireRate;
            }
        }

        public void Init(IEcsSystems systems)
        {
            
        }
    }
}