using Leopotam.EcsLite;
using td.components.behaviors;
using td.components.flags;
using td.components.refs;
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
            world.GetComponent<LinearMovementToTarget>(entity).gap = Constants.DefaultGap;
            
            world.DelComponent<IsDisabled>(entity);
            world.DelComponent<IsDestroyed>(entity);

#if UNITY_EDITOR
            if (!gameObject.GetComponent<EcsComponentsInfo>())
            {
                gameObject.AddComponent<EcsComponentsInfo>();
            }
#endif
        }
    }
}