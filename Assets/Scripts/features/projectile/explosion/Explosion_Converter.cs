using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy;
using td.features.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile.explosion
{
    public class Explosion_Converter : BaseEntity_Converter
    {
        [DI] private Projectile_Aspect projectileAspect;
        [DI] private Explosion_Aspect explosionAspect;
        [DI] private Destroy_Service destroyService;

        public override ProtoWorld World() => projectileAspect.World();

        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);

            projectileAspect.explosiveAttributePool.GetOrAdd(entity);
            explosionAspect.explosionPool.GetOrAdd(entity);
            explosionAspect.refExplosionMBPool.GetOrAdd(entity).reference = gameObject.GetComponent<ExplosionMonoBehaviour>();
            destroyService.SetIsOnlyOnLevel(entity, true);
        }
    }
}