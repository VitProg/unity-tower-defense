using Leopotam.EcsLite;
using NaughtyAttributes;
using td.features.infoPanel.bus;
using td.features.inputEvents;
using td.features.shard;
using td.monoBehaviours;
using td.utils.di;
using UnityEngine;
using td.features._common.components;
#if UNITY_EDITOR
using UnityEditor;
using Leopotam.EcsLite.UnityEditor;
#endif

namespace td.features.tower.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    public class ShardTowerMonoBehaviour : MonoInjectable, IInputEventsHandler
    {
        [Required] public EcsEntity ecsEntity;

        private readonly EcsInject<Shard_Service> shardServise;
        private readonly EcsInject<IEventBus> events;

        // private void Update()
        // {
            // shardService.Value.GetShardInTower()
        // }

        public void OnPointerEnter(float x, float y)
        {
            // if (ecsEntity.packedEntity.HasValue)
            // events.Value.Global.Add<>()
            if (ecsEntity.packedEntity.HasValue)
            {
                events.Value.Global.Add<Command_ShowTowerInfo>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        public void OnPointerLeave(float x, float y)
        {
            if (ecsEntity.packedEntity.HasValue)
            {
                events.Value.Global.Add<Command_HideTowerInfo>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        public void OnPointerDown(float x, float y)
        {
            // throw new NotImplementedException();
        }

        public void OnPointerUp(float x, float y, bool inRadius)
        {
            // throw new NotImplementedException();
        }

        public void OnPointerClick(float x, float y)
        {
            // throw new NotImplementedException();
            if (ecsEntity.packedEntity.HasValue)
            {
                events.Value.Global.Add<Command_ShowTowerInfo>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
    }
}