using Leopotam.EcsProto.QoL;
using NaughtyAttributes;
using td.features.infoPanel.bus;
using td.features.inputEvents;
using td.features.shard;
using td.monoBehaviours;
using td.utils.di;
using UnityEngine;
using td.features.eventBus;

namespace td.features.tower.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    public class ShardTowerMonoBehaviour : MonoBehaviour, IInputEventsHandler
    {
        [Required] public EcsEntity ecsEntity;

        private Shard_Service ShardService =>  ServiceContainer.Get<Shard_Service>();
        private EventBus Events =>  ServiceContainer.Get<EventBus>();

        public void OnPointerEnter(float x, float y)
        {
            if (ecsEntity.packedEntity.HasValue)
            {
                Events.global.Add<Command_ShowTowerInfo>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        public void OnPointerLeave(float x, float y)
        {
            if (ecsEntity.packedEntity.HasValue)
            {
                Events.global.Add<Command_HideTowerInfo>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        public void OnPointerDown(float x, float y)
        {
        }

        public void OnPointerUp(float x, float y, bool inRadius)
        {
        }

        public void OnPointerClick(float x, float y)
        {
            if (ecsEntity.packedEntity.HasValue)
            {
                Events.global.Add<Command_ShowTowerInfo>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
    }
}