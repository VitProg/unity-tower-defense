using Leopotam.EcsLite;
using NaughtyAttributes;
using td.monoBehaviours;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.towers.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    public class TowerMonoBehaviour : MonoBehaviour, IConvertibleToEntity
    {   
        [OnValueChanged("OnValueChanged")]
        public float radius;
        
        [OnValueChanged("OnValueChanged")]
        public int cost;

        public GameObject barrel;
        
        private EcsEntity ecsEntity;

        private void Start()
        {
            ecsEntity = GetComponent<EcsEntity>();
        }
        
        public void UpdateEntity(EcsWorld world, int entity)
        {
            ref var tower = ref world.GetComponent<Tower>(entity);
            tower.radius = radius;
            tower.cost = cost;
            tower.barrel = barrel ? barrel.transform.localPosition : new Vector2(0, 0);
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
            if (ecsEntity != null && ecsEntity.TryGetEntity(out var entity) && world.HasComponent<Tower>(entity))
            {
                ref var tower = ref world.GetComponent<Tower>(entity);
                radius = tower.radius;
                cost = tower.cost;
                if (barrel)
                {
                    barrel.transform.localPosition = tower.barrel;
                }
            }
        }
#endif
    }
}