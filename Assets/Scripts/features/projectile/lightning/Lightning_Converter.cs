using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy;
using td.features.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile.lightning
{
    public class Lightning_Converter : BaseEntity_Converter
    {
        [DI] private Lightning_Aspect lightningAspect;
        [DI] private Projectile_Aspect projectileAspect;
        [DI] private Destroy_Service destroyService;

        public override ProtoWorld World() => projectileAspect.World();

        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);

            lightningAspect.lightningPool.GetOrAdd(entity);
            lightningAspect.refLineRendererPool.GetOrAdd(entity).reference = gameObject.GetComponent<LineRenderer>();
            projectileAspect.lightningAttributePool.GetOrAdd(entity);
            destroyService.SetIsOnlyOnLevel(entity, true);
            destroyService.SetIsDisabled(entity, false);
            destroyService.SetIsDestroyed(entity, false);
        }
    }
}