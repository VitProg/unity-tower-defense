using Leopotam.EcsLite;
using NaughtyAttributes;
using td.common;
using td.monoBehaviours;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.towers.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    [SelectionBase]
    public class TowerMonoBehaviour : MonoBehaviour, IConvertibleToEntity, ISortingLayerChangeable
    {   
        public GameObject barrel;
        public SpriteRenderer sprite;
        public EcsEntity ecsEntity;

        [OnValueChanged("OnValueChanged")]
        public float radius;
        
        [OnValueChanged("OnValueChanged")]
        public int cost;

        [InfoBox("Do not change sortingLayer on play state!")]
        [SerializeField][SortingLayer] private string _sortingLayer = Constants.Layers.L3_Buildings;
        
        public string sortigLayer
        {
            get => _sortingLayer;
            set
            {
                _sortingLayer = value;
                if (sprite)
                {
                    sprite.sortingLayerName = _sortingLayer;
                }
            }
        }


        private void Start()
        {
            // ecsEntity ??= GetComponent<EcsEntity>();
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