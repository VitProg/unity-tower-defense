using System;
using Leopotam.EcsLite;
using td.components.flags;
using td.components.refs;
using td.features.towers.mb;
using td.monoBehaviours;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.towers
{
    //ToDo
    public class TowerEntityConverter : IEntityConverter<Tower>
    {
        [InjectWorld] private EcsWorld world;
        
        public void Convert(GameObject gameObject, int entity)
        {
            world.GetComponent<Tower>(entity);
            world.GetComponent<OnlyOnLevel>(entity);
            world.GetComponent<Ref<GameObject>>(entity).reference = gameObject;
            
            world.DelComponent<IsDisabled>(entity);
            world.DelComponent<IsDestroyed>(entity);
            
            if (!gameObject.GetComponent<HexGridSnaping>())
            {
                gameObject.AddComponent<HexGridSnaping>();
            }
#if UNITY_EDITOR
            if (!gameObject.GetComponent<EcsComponentsInfo>())
            {
                gameObject.AddComponent<EcsComponentsInfo>();
            }
#endif
            if (gameObject.TryGetComponent(out TowerMonoBehaviour tower))
            {
                tower.UpdateEntity(world, entity);
            }
            if (gameObject.TryGetComponent(out CannonTowerMonoBehaviour cannonTower))
            {
                cannonTower.UpdateEntity(world, entity);
            }
            if (gameObject.TryGetComponent(out ShardTowerMonoBehaviour shardTower))
            {
                shardTower.UpdateEntity(world, entity);
            }
        }
    }
}