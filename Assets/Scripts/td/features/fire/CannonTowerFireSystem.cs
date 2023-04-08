using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.attributes;
using td.components.behaviors;
using td.components.flags;
using td.components.links;
using td.features.enemies;
using td.features.fire.components;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fire
{
    public class CannonTowerFireSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<IsCannonTower, IsTower, GameObjectLink>> entities = default;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (var entity in entities.Value)
            {
                ref var connon = ref entities.Pools.Inc1.Get(entity);
                ref var tower = ref entities.Pools.Inc2.Get(entity);
                var connonGameObject = entities.Pools.Inc3.Get(entity);

                connon.fireCountdown -= Time.deltaTime;
                
                var lunchProjectile = false;
                
                if (connon.fireCountdown < Constants.ZeroFloat)
                {
                    connon.fireCountdown = 1f / connon.fireRate;
                    lunchProjectile = true;
                }

                if (!lunchProjectile ||
                    !world.HasComponent<FireTarget>(entity))
                {
                    continue;
                }

                var target = world.GetComponent<FireTarget>(entity);

                if (target.TargetEntity.Unpack(world, out var enemyEntity))
                {
                    var enemyGameObject = world.GetComponent<GameObjectLink>(enemyEntity);
                    var enemyTarget = world.GetComponent<Target>(enemyEntity);
                    var enemyState = world.GetComponent<EnemyState>(enemyEntity);
                    
                    var enemyPostiion = enemyGameObject.gameObject.transform.position;

                    var projectilePosition = connonGameObject.gameObject.transform.position;
                    var projectileTarget = enemyPostiion;

                    var distance = (projectilePosition - projectileTarget).magnitude;

                    if (distance > tower.radius)
                    {
                        continue;
                    }

                    var enemyVector = (Vector3)enemyTarget.target - enemyPostiion;
                    enemyVector.Normalize();
                    enemyVector *= ((enemyState.speed / 2f) + (connon.projectileSpeed / 2f)) * (distance / 10f);

                    projectileTarget += enemyVector; //todo
                    
                    var projectileGameObject = Object.Instantiate(
                        (GameObject)Resources.Load("Prefabs/projectiles/bullet", typeof(GameObject)),
                        projectilePosition,
                        new Quaternion(0, 0, 0, 0),
                        connonGameObject.gameObject.transform
                    );
                    var projectileEntity = world.ConvertToEntity(projectileGameObject);
                    
                    world.GetComponent<IsProjectile>(projectileEntity).damage = connon.damage;
                    world.GetComponent<MoveToTarget>(projectileEntity).speed = connon.projectileSpeed;
                    world.AddComponent(projectileEntity, new Target()
                    {
                        target = projectileTarget,
                        gap = Constants.DefaultGap,
                    });

                    world.AddComponent(projectileEntity, new FireTarget()
                    {
                        TargetEntity = world.PackEntity(enemyEntity),
                    });
                }
            }
        }
    }
}