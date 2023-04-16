using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.components.behaviors;
using td.components.flags;
using td.features.enemies;
using td.features.fire;
using td.features.fire.projectiles;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.towers
{
    public class CannonTowerFireSystem : IEcsRunSystem
    {
        [EcsInject] private LevelMap levelMap;
        
        private readonly EcsFilterInject<
            Inc<CannonTower, Tower, Ref<GameObject>>,
            Exc<IsDragging>
        > cannonEntities = default;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

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
                    !world.HasComponent<FireTarget>(cannonEntity))
                {
                    continue;
                }

                var target = world.GetComponent<FireTarget>(cannonEntity);

                if (target.TargetEntity.Unpack(world, out var enemyEntity))
                {
                    var enemy = world.GetComponent<Enemy>(enemyEntity);
                    var enemyGameObject = world.GetComponent<Ref<GameObject>>(enemyEntity);
                    var enemyTarget = world.GetComponent<LinearMovementToTarget>(enemyEntity);

                    var enemyPostiion = enemyGameObject.reference.transform.position;

                    var projectilePosition = connonGameObject.reference.transform.position;
                    var projectileTarget = enemyPostiion;

                    var distance = (projectilePosition - projectileTarget).magnitude;

                    if (distance > tower.radius)
                    {
                        continue;
                    }

                    var enemyVector = (Vector3)enemyTarget.target - enemyPostiion;
                    enemyVector.Normalize();
                    enemyVector *= ((enemy.speed / 2f) + (cannon.projectileSpeed / 2f)) * (distance / 10f);

                    projectileTarget += enemyVector; //todo

                    var projectileGameObject = Object.Instantiate(
                        (GameObject)Resources.Load("Prefabs/projectiles/bullet", typeof(GameObject)),
                        projectilePosition,
                        new Quaternion(0, 0, 0, 0),
                        connonGameObject.reference.transform
                    );
                    projectileGameObject.transform.localScale = Vector2.one * levelMap.CellSize;
                    var projectileEntity = world.ConvertToEntity(projectileGameObject);

                    world.GetComponent<IsProjectile>(projectileEntity).WhoFired = world.PackEntity(cannonEntity);
                    
                    // todo применять различный тип снаряда, в зависимости от находящихся внутри осколков
                    world.AddComponent(projectileEntity, new DamageProjectile()
                    {
                        damage = cannon.damage,
                    });
                    world.AddComponent(projectileEntity, new SlowingProjectile()
                    {
                        duration = 5f,
                        speedMultipler = 1.5f
                    });
                    world.AddComponent(projectileEntity, new PoisonProjectile()
                    {
                        duration = 8f,
                        damageInterval = cannon.damage,
                        interval = 1f,
                    });
                    // todo ^
                    
                    world.AddComponent<LinearMovementToTarget>(projectileEntity) = new LinearMovementToTarget()
                    {
                        target = projectileTarget,
                        speed = cannon.projectileSpeed,
                        gap = Constants.DefaultGap,
                    };

                    world.AddComponent(projectileEntity, new FireTarget()
                    {
                        TargetEntity = world.PackEntity(enemyEntity),
                    });
                    
                    cannon.fireCountdown = 1f / cannon.fireRate;
                }
            }
        }
    }
}