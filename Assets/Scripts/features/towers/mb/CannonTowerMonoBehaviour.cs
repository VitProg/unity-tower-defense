using Leopotam.EcsLite;
using NaughtyAttributes;
using td.monoBehaviours;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.towers.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    public class CannonTowerMonoBehaviour : MonoBehaviour, IConvertibleToEntity
    {
        [OnValueChanged("OnValueChanged")]
        public float damage;
        
        [OnValueChanged("OnValueChanged")]
        public float fireRate;
        
        [OnValueChanged("OnValueChanged")]
        public float projectileSpeed;

        private EcsEntity ecsEntity;

        private void Start()
        {
            ecsEntity = GetComponent<EcsEntity>();
        }
        
        public void UpdateEntity(EcsWorld world, int entity)
        {
            ref var cannon = ref world.GetComponent<CannonTower>(entity);
            cannon.damage = damage;
            cannon.fireRate = fireRate;
            cannon.projectileSpeed = projectileSpeed;
        }
        
#if UNITY_EDITOR
        private void OnValueChanged()
        {
            if (ecsEntity != null && ecsEntity.TryGetEntity(out var entity))
            {
                UpdateEntity(DI.GetWorld(), entity);
            }
        }

        [Button("Update fields from Entity", EButtonEnableMode.Playmode)]
        public void UpdateFromEntity()
        {
            var world = DI.GetWorld();
            if (ecsEntity != null && ecsEntity.TryGetEntity(out var entity) && world.HasComponent<CannonTower>(entity))
            {
                ref var tower = ref world.GetComponent<CannonTower>(entity);
                damage = tower.damage;
                fireRate = tower.fireRate;
                projectileSpeed = tower.projectileSpeed;
            }
        }
#endif
    }
}