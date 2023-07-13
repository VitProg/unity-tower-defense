using Leopotam.EcsLite;
using td.components.flags;
using td.components.refs;
using td.features.projectiles.attributes;
using td.monoBehaviours;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles.lightning
{
    public class LightningLineEntityConverter : IEntityConverter<LightningLine>
    {
        [InjectWorld] private EcsWorld world;
        
        public void Convert(GameObject gameObject, int entity)
        {
            world.GetComponent<LightningLine>(entity);
            world.GetComponent<LightningAttribute>(entity);
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