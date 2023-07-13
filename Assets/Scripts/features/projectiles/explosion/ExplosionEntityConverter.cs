using Leopotam.EcsLite;
using td.components.flags;
using td.components.refs;
using td.features.projectiles.attributes;
using td.features.projectiles.explosion;
using td.monoBehaviours;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles.explosion
{
    public class ExplosionEntityConverter : IEntityConverter<Explosion>
    {
        [InjectWorld] private EcsWorld world;
        
        public void Convert(GameObject gameObject, int entity)
        {
            world.GetComponent<Explosion>(entity);
            world.GetComponent<ExplosiveAttribute>(entity);
            world.GetComponent<OnlyOnLevel>(entity);
            world.GetComponent<Ref<GameObject>>(entity).reference = gameObject;

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