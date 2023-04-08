// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
// using td.components.attributes;
// using td.components.behaviors;
// using td.components.flags;
// using td.components.links;
// using td.features.enemies;
// using td.features.fire.components;
// using td.utils.ecs;
// using UnityEngine;
//
// namespace td.features.fire
// {
//     public class RocketTowerFireSystem : IEcsRunSystem
//     {
//         private readonly EcsFilterInject<Inc<IsRocketTower, IsTower, GameObjectLink>> entities = default;
//
//         public void Run(IEcsSystems systems)
//         {
//             var world = systems.GetWorld();
//
//             foreach (var entity in entities.Value)
//             {
//                 ref var rocket = ref entities.Pools.Inc1.Get(entity);
//                 ref var tower = ref entities.Pools.Inc2.Get(entity);
//                 ref var connonGameObject = ref entities.Pools.Inc3.Get(entity);
//
//                 rocket.fireCountdown -= Time.deltaTime;
//
//                 var lunchProjectile = false;
//                 
//                 if (rocket.fireCountdown < Constants.ZeroFloat)
//                 {
//                     rocket.fireCountdown = 1f / rocket.fireRate;
//                     lunchProjectile = true;
//                 }
//
//                 if (!lunchProjectile ||
//                     !world.HasComponent<FireTarget>(entity))
//                 {
//                     continue;
//                 }
//
//                 var target = world.GetComponent<FireTarget>(entity);
//
//                 if (target.TargetEntity.Unpack(world, out var targetEntity))
//                 {
//                     // var targetEnemy = world.GetComponent<IsEnemy>(targetEntity);
//                     
//                     var enemyGameObject = world.GetComponent<GameObjectLink>(targetEntity);
//                     var enemyTarget = world.GetComponent<Target>(targetEntity);
//                     var enemyPostiion = enemyGameObject.gameObject.transform.position;
//                     var enemyStats = world.GetComponent<SpawnEnemyCommand>(targetEntity);
//
//                     var projectilePosition = connonGameObject.gameObject.transform.position;
//                     var projectileTarget = enemyPostiion;
//
//                     var distance = (projectilePosition - projectileTarget).magnitude;
//
//                     if (distance > tower.radius)
//                     {
//                         continue;
//                     }
//
//                     // var enemyVector = (Vector3)enemyTarget.target - enemyPostiion;
//                     // enemyVector.Normalize();
//                     // enemyVector *= ((enemyStats.speed / 2f) + (rocket.maxEnergy / 2f)) * (distance / 10f);
//
//                     // projectileTarget += enemyVector; //todo
//                     
//                     var projectileGameObject = Object.Instantiate(
//                         (GameObject)Resources.Load("Prefabs/projectiles/rocket", typeof(GameObject)),
//                         projectilePosition,
//                         new Quaternion(0, 0, 0, 0),
//                         connonGameObject.gameObject.transform
//                     );
//                     var projectileEntity = UniEcsUtils.Convert(projectileGameObject, world);
//                     
//                     world.GetComponent<IsProjectile>(projectileEntity).damage = rocket.damage;
//                     world.GetComponent<MoveToTarget>(projectileEntity).speed = rocket.maxEnergy;
//                     world.AddComponent(projectileEntity, new Target()
//                     {
//                         target = projectileTarget,
//                         gap = Constants.DefaultGap,
//                     });
//                     world.AddComponent(projectileEntity, new FireTarget()
//                     {
//                         TargetEntity = world.PackEntity(targetEntity),
//                     });
//                     world.AddComponent(projectileEntity, new InertiaOfMovement()
//                     {
//                        turnSpeed = rocket.turnSpeed,
//                        maxEnergy = rocket.maxEnergy,
//                        energyUsage = 0f,
//                        currentEnergy = rocket.maxEnergy,
//                        maxSpeed = rocket.maxSpeed
//                     });
//                 }
//             }
//         }
//     }
// }