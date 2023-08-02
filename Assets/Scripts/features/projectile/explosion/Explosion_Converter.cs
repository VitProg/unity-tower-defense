using Leopotam.EcsLite;
using td.features._common;
using td.features.ecsConverter;
using UnityEngine;

namespace td.features.projectile.explosion
{
    public class Explosion_Converter : BaseEntity_Converter
    {
        private readonly EcsInject<Projectile_Service> projectileService;
        private readonly EcsInject<Common_Service> common;
        
        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);
            
            projectileService.Value.GetExplosion(entity);
            projectileService.Value.GetExplosiveAttribute(entity);
            common.Value.SetIsOnlyOnLevel(entity, true);
        }
    }
}