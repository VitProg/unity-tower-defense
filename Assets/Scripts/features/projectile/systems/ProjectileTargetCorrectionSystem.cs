using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features._common.flags;
using td.features.enemy;
using td.features.projectile.components;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile.systems
{
    public class ProjectileTargetCorrectionSystem : EcsIntervalableRunSystem
    {
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<Common_Service> common;
        
        private readonly EcsFilterInject<
            Inc<Projectile, ProjectileTarget, MovementToTarget, ObjectTransform>, 
            Exc<IsDestroyed, IsDisabled>
        > projectileEntities = default;

        public override void IntervalRun(IEcsSystems systems, float deltaTime)
        {
            var world = systems.GetWorld();
            
            foreach (var projectileEntity in projectileEntities.Value)
            {
                ref var fireTarget = ref projectileEntities.Pools.Inc2.Get(projectileEntity);
                ref var movement = ref projectileEntities.Pools.Inc3.Get(projectileEntity);
                ref var transform = ref projectileEntities.Pools.Inc4.Get(projectileEntity);

                if (
                    !fireTarget.targetEntity.Unpack(world, out var targetEntity) ||
                    !enemyService.Value.IsAlive(targetEntity)
                )
                {
                    Debug.Log("Projectile don't have fire target. Destroy it.");
                    
                    projectileEntities.Pools.Inc2.SafeDel(projectileEntity);
                    projectileEntities.Pools.Inc3.SafeDel(projectileEntity);
                    common.Value.SafeDelete(projectileEntity);
                    continue;
                }

                if (common.Value.HasTransform(targetEntity))
                {
                    if (common.Value.HasMovement(targetEntity))
                    {
                        var targetPos = common.Value.GetTransform(targetEntity).position;
                        var d = (transform.position - targetPos).magnitude; // todo optimize
                        var targetSpeedV = common.Value.GetMovement(targetEntity).speedV / movement.speed * d;
                        var targetPoint = targetPos + targetSpeedV;
                        movement.target = targetPoint;
                    }
                    else
                    {
                        movement.target = common.Value.GetTransform(targetEntity).position;
                    }
                    movement.SetSpeed(movement.speed, movement.target - movement.from);

                    //todo
                    // var fromToTargetV = (movement.target - movement.from);
                    // movement.fromToTargetDistanse = fromToTargetV.magnitude;
                    // enemy.distanceFromSpawn += movement.fromToTargetDistanse;
                    //
                    // var norm = fromToTargetV / movement.fromToTargetDistanse;
                    // movement.speedV.x = norm.x * movement.speed;
                    // movement.speedV.y = norm.y * movement.speed;
                }
                else if (!common.Value.HasGameObject(targetEntity, true))
                {
                    movement.target = common.Value.GetGOPosition(targetEntity);
                }
            }
        }
        
        public ProjectileTargetCorrectionSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}