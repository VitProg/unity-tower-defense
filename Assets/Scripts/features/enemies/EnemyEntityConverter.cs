using System;
using Leopotam.EcsLite;
using td.components.flags;
using td.components.refs;
using td.features.enemies.components;
using td.features.impactsEnemy;
using td.monoBehaviours;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies
{
    public class EnemyEntityConverter : IEntityConverter<Enemy>
    {
        [InjectWorld] private EcsWorld world;
        
        public void Convert(GameObject gameObject, int entity)
        {
            world.GetComponent<Enemy>(entity);
            world.GetComponent<OnlyOnLevel>(entity);
            world.GetComponent<Ref<GameObject>>(entity).reference = gameObject;
            
            world.DelComponent<IsDisabled>(entity);
            world.DelComponent<IsDestroyed>(entity);
            
            world.DelComponent<IsFreezed>(entity);
            
            world.DelComponent<PoisonDebuff>(entity);
            world.DelComponent<ShockingDebuff>(entity);
            world.DelComponent<SpeedDebuff>(entity);
            
#if UNITY_EDITOR
            if (!gameObject.GetComponent<EcsComponentsInfo>())
            {
                gameObject.AddComponent<EcsComponentsInfo>();
            }
#endif
        }
    }
}