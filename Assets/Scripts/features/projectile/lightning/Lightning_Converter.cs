using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.ecsConverter;
using UnityEngine;

namespace td.features.projectile.lightning
{
    public class LightningLine_Converter : BaseEntity_Converter
    {
        private readonly EcsInject<Projectile_Service> projectileService;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsWorldInject world;

        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);
            
            projectileService.Value.GetLightningLine(entity);
            projectileService.Value.GetLightningAttribute(entity);
            common.Value.SetIsOnlyOnLevel(entity, true);
            common.Value.SetIsDisabled(entity, false);
            common.Value.SetIsDestroyed(entity, false);
        }
    }
}