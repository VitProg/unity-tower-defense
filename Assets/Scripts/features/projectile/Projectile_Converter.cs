using Leopotam.EcsLite;
using td.features._common;
using td.features.ecsConverter;
using UnityEngine;

namespace td.features.projectile
{
    public class Projectile_Converter : BaseEntity_Converter
    {
        private readonly EcsInject<Projectile_Service> projectileService;
        private readonly EcsInject<Common_Service> common;
        
        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);
            
            projectileService.Value.GetProjectile(entity);
            projectileService.Value.GetTarget(entity);
            common.Value.SetIsOnlyOnLevel(entity, true);

            ref var movement = ref common.Value.GetMovement(entity);
            movement.gapSqr = Constants.DefaultGapSqr;
            movement.speedOfGameAffected = true;

            common.Value.SetIsDisabled(entity, false);
            common.Value.SetIsDestroyed(entity, false);
            projectileService.Value.RemoveAllAttributes(entity);
        }
    }
}