using System.Runtime.CompilerServices;
using NaughtyAttributes;
using td.features.eventBus;
using td.features.infoPanel.bus;
using td.features.inputEvents;
using td.features.shard;
using td.features.tower.towerMenu.bus;
using td.monoBehaviours;
using td.utils.di;
using UnityEngine;

namespace td.features.tower.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    public class ShardTowerMonoBehaviour : MonoBehaviour, IInputEventsHandler
    {
        [Required] public EcsEntity ecsEntity;

        private Shard_Service ShardService =>  ServiceContainer.Get<Shard_Service>();
        private EventBus Events =>  ServiceContainer.Get<EventBus>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnPointerEnter(float x, float y)
        {
            if (ecsEntity.packedEntity.HasValue)
            {
                Events.global.Add<Command_ShowTowerInfo>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnPointerLeave(float x, float y)
        {
            if (ecsEntity.packedEntity.HasValue)
            {
                Events.global.Add<Command_HideTowerInfo>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnPointerDown(float x, float y)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnPointerUp(float x, float y, bool inside)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnPointerClick(float x, float y, bool isLong)
        {
            if (ecsEntity.packedEntity.HasValue)
            {
                if (isLong) Events.global.Add<Command_ShowTowerMenu>().towerEntity = ecsEntity.packedEntity.Value;
                else Events.global.Add<Command_ShowTowerInfo>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }
        
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public float TimeFromDown { get; set; }
    }
}