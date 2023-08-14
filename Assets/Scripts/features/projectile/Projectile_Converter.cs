using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy;
using td.features.ecsConverter;
using td.features.movement;
using UnityEngine;

namespace td.features.projectile
{
    public class Projectile_Converter : BaseEntity_Converter
    {
        [DI] private Projectile_Aspect aspect;
        [DI] private Projectile_Service projectileService;
        [DI] private Destroy_Service destroyService;
        [DI] private Movement_Service movementService;

        public override ProtoWorld World() => aspect.World();

        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);
            
            projectileService.GetProjectile(entity);
            projectileService.GetTarget(entity);
            destroyService.SetIsOnlyOnLevel(entity, true);

            ref var movement = ref movementService.GetMovement(entity);
            movement.gapSqr = Constants.DefaultGapSqr;
            movement.speedOfGameAffected = true;
            
            movementService.SetCustomMovement(entity, true);

            destroyService.SetIsDisabled(entity, false);
            destroyService.SetIsDestroyed(entity, false);
            projectileService.RemoveAllAttributes(entity);
        }
    }
}