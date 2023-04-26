// using System;
// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
// using td.components.behaviors;
// using td.components.flags;
// using td.services;
// using td.services.ecsConverter;
// using td.utils.ecs;
// using Object = UnityEngine.Object;
//
// namespace td.features.projectiles
// {
//     public class SpawnProjectileExecuter : IEcsRunSystem
//     {
//         [Inject] private GameObjectPoolService poolService;
//         [Inject] private EntityConverters converters;
//         [Inject] private ProjectileService projectileService;
//         [InjectWorld] private EcsWorld world;
//
//         private readonly EcsFilterInject<Inc<SpawnProjectileOuterCommand>> entities = Constants.Worlds.Outer;
//
//         public void Run(IEcsSystems systems)
//         {
//             foreach (var commandEntity in entities.Value)
//             {
//                 ref var spawnCommand = ref entities.Pools.Inc1.Get(commandEntity);
//
//                 var projectile = projectileService.CreateObject(spawnCommand.prefab, spawnCommand.startedPosition);
//
//                 if (!converters.Convert<Projectile>(projectile.gameObject, out var projectileEntity))
//                 {
//                     throw new NullReferenceException($"Failed to convert GameObject {projectile.gameObject.name}");
//                 }
//
//                 world.GetComponent<Projectile>(projectileEntity).WhoFired = spawnCommand.WhoFired;
//
//                 projectileService.ApplyCommandAttributes(commandEntity, projectileEntity);
//                 
//                 ref var movement = ref world.GetComponent<LinearMovementToTarget>(projectileEntity);
//                 movement.target = spawnCommand.targetPosition;
//                 movement.speed = spawnCommand.speed;
//                 movement.gap = Constants.DefaultGap;
//
//                 world.GetComponent<ProjectileTarget>(projectileEntity).TargetEntity = spawnCommand.TargetEntity;
//
//                 world.DelComponent<IsDisabled>(projectileEntity);
//                 world.DelComponent<IsDestroyed>(projectileEntity);
//             }
//         }
//     }
// }