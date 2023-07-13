using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.behaviors;
using td.components.flags;
using td.components.refs;
using td.features.dragNDrop;
using td.features.enemies.components;
using td.features.projectiles;
using td.features.projectiles.attributes;
using td.features.shards;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.towers
{
    public class CannonTowerFireSystem : IEcsRunSystem
    {
        [Inject] private LevelMap levelMap;
        [Inject] private ProjectileService projectileService;
        [InjectWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<
            Inc<CannonTower, Tower, Ref<GameObject>>,
            Exc<IsDragging, IsDisabled, IsDestroyed>
        > cannonEntities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var cannonEntity in cannonEntities.Value)
            {
                ref var cannon = ref cannonEntities.Pools.Inc1.Get(cannonEntity);
                ref var tower = ref cannonEntities.Pools.Inc2.Get(cannonEntity);
                var connonGameObject = cannonEntities.Pools.Inc3.Get(cannonEntity);

                var lunchProjectile = false;

                if (cannon.fireCountdown > 0)
                {
                    cannon.fireCountdown -= Time.deltaTime;
                }

                if (cannon.fireCountdown < Constants.ZeroFloat)
                {
                    lunchProjectile = true;
                }

                if (!lunchProjectile ||
                    !world.HasComponent<ProjectileTarget>(cannonEntity))
                {
                    continue;
                }

                var sqrRadius = tower.radius * tower.radius;
                
                var target = world.GetComponent<ProjectileTarget>(cannonEntity);

                if (target.targetEntity.Unpack(world, out var enemyEntity))
                {
                    if (world.HasComponent<IsDisabled>(enemyEntity) ||
                        world.HasComponent<IsDestroyed>(enemyEntity))
                    {
                        continue;
                    }
                    
                    ref var enemyGameObject = ref world.GetComponent<Ref<GameObject>>(enemyEntity);
                    
                    var enemyPostiion = (Vector2)enemyGameObject.reference.transform.position;

                    var projectilePosition = (Vector2)connonGameObject.reference.transform.position + tower.barrel;
                    var projectileTarget = enemyPostiion;

                    var sqrDistance = (projectilePosition - projectileTarget).sqrMagnitude;

                    if (sqrDistance > sqrRadius)
                    {
                        continue;
                    }
                    
                    ///
                    var shard = new Shard();

                    var projectileEntity = projectileService.SpawnProjectile(
                        name: "bullet",
                        position: projectilePosition,
                        targetEntity: enemyEntity,
                        speed: cannon.projectileSpeed,
                        whoFired: cannonEntity,
                        ref shard
                    );
                    
                    // start todo
                    ref var damage = ref world.GetComponent<DamageAttribute>(projectileEntity);
                    damage.damage = cannon.damage; // todo add random damage in range
                    
                    ref var slowing = ref world.GetComponent<SlowingAttribute>(projectileEntity);
                    slowing.duration = 5f;
                    slowing.speedMultipler = 1.5f;
                    
                    ref var poison = ref world.GetComponent<PoisonAttribute>(projectileEntity);
                    poison.duration = 8f;
                    poison.damageInterval = cannon.damage / 5f;
                    poison.interval = 1f;
                    // end todo
                    
                    cannon.fireCountdown = 1f / cannon.fireRate;
                }
            }
        }
    }
}