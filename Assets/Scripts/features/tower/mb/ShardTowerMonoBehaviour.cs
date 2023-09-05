using System.Runtime.CompilerServices;
using NaughtyAttributes;
using td.features._common.interfaces;
using td.features.eventBus;
using td.features.level;
using td.features.shard;
using td.features.tower.bus;
using td.utils;
using td.utils.di;
using td.utils.ecs;
using UnityEngine;

namespace td.features.tower.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    public class ShardTowerMonoBehaviour : MonoBehaviour, IInputEventsHandler
    {
        [Required] public EcsEntity ecsEntity;
        [Required] public GameObject barrel;
        [Required] public SpriteRenderer sprite;
        [Required] public LineRenderer radiusRenderer;
        
        [ShowNativeProperty]public int CellCoordX => HexGridUtils.PositionToCell(transform.position).x;
        [ShowNativeProperty]public int CellCoordY => HexGridUtils.PositionToCell(transform.position).y;

        #region DI
        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnPointerEnter(float x, float y)
        {
            Debug.Log(">>> ShardTowerMB OnPointerEnter"+ecsEntity.packedEntity);
            Events.global.Add<Event_Tower_Hovered>().Tower = ecsEntity.packedEntity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnPointerLeave(float x, float y)
        {
            Debug.Log(">>> ShardTowerMB OnPointerLeave"+ecsEntity.packedEntity);
            Events.global.Add<Event_Tower_UnHovered>().Tower = ecsEntity.packedEntity;
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
            ref var ev = ref Events.global.Add<Event_Tower_Clicked>();
            ev.Tower = ecsEntity.packedEntity;
            ev.isLong = isLong;
        }
        
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public float TimeFromDown { get; set; }
    }
}