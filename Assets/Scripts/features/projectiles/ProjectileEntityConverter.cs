using Leopotam.EcsLite;
using td.components.behaviors;
using td.components.flags;
using td.components.refs;
using td.features.projectiles.attributes;
using td.monoBehaviours;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles
{
    public class ProjectileEntityConverter : IEntityConverter<Projectile>
    {
        [InjectWorld] private EcsWorld world;
        
        public void Convert(GameObject gameObject, int entity)
        {
            world.GetComponent<Projectile>(entity);
            world.GetComponent<ProjectileTarget>(entity);
            world.GetComponent<OnlyOnLevel>(entity);
            world.GetComponent<Ref<GameObject>>(entity).reference = gameObject;

            ref var movement = ref world.GetComponent<LinearMovementToTarget>(entity);
            movement.gap = Constants.DefaultGap;
            movement.speedOfGameAffected = true;
            
            world.DelComponent<IsDisabled>(entity);
            world.DelComponent<IsDestroyed>(entity);
            
            world.DelComponent<DamageAttribute>(entity);
            world.DelComponent<ExplosiveAttribute>(entity);
            world.DelComponent<LightningAttribute>(entity);
            world.DelComponent<PoisonAttribute>(entity);
            world.DelComponent<ShockingAttribute>(entity);
            world.DelComponent<SlowingAttribute>(entity);

#if UNITY_EDITOR
            if (!gameObject.GetComponent<EcsComponentsInfo>())
            {
                gameObject.AddComponent<EcsComponentsInfo>();
            }
#endif
        }
    }
}