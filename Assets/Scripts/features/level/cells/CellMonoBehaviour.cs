using System.Runtime.CompilerServices;
using NaughtyAttributes;
using td.features.eventBus;
using td.features.inputEvents;
using td.features.level.bus;
using td.utils;
using td.utils.di;
using UnityEngine;

namespace td.features.level.cells
{
#if UNITY_EDITOR
    [SelectionBase] // Если вы кликнете на внутреннюю запчасть префаба, то выделится именно этот объект
#endif
    public class CellMonoBehaviour : MonoBehaviour, IInputEventsHandler
    {
        #region properties

        [Required] public SpriteRenderer plus;
        
        [Space]
        [BoxGroup("Main Parapeters")] public CellTypes type;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)]
        public bool isAutoNextSearching = true;

        [Space] [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)]
        public bool isSwitcher;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)]
        public HexDirections directionToNext = HexDirections.NONE;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)] [EnableIf("isSwitcher")]
        public HexDirections directionToAltNext = HexDirections.NONE;
        
        // [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)]
        // public HexDirections directionToPrev = HexDirections.NONE;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)] [DisableIf("isSpawn")]
        public bool isKernel;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)] [DisableIf("isKernel")]
        public bool isSpawn;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)] [EnableIf("isKernel")]
        public byte kernelNumber = 0;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)] [EnableIf("isSpawn")]
        public byte spawnNumber = 0;
        #endregion

        private EventBus Events =>  ServiceContainer.Get<EventBus>();
        
        //
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref Cell GetCell() => ref ServiceContainer.Get<LevelMap>().GetCell(transform.position);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnPointerEnter(float x, float y)
        {
            ref var cell = ref GetCell();
            if (cell.type == CellTypes.CanBuild && !cell.HasBuilding())
            {
                plus.gameObject.SetActive(true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnPointerLeave(float x, float y)
        {
            ref var cell = ref GetCell();
            if (cell.type == CellTypes.CanBuild && !cell.HasBuilding())
            {
                plus.gameObject.SetActive(false);
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
            ref var cell = ref GetCell();
            if (cell.type == CellTypes.CanBuild)
            {
                ref var ev = ref Events.global.Add<Event_CellCanBuild_Clicked>();
                ev.coords = cell.coords;
                ev.isLong = isLong;
            }

            if (cell.type == CellTypes.CanWalk)
            {
                ref var ev = ref Events.global.Add<Event_CellCanWalk_Clicked>();
                ev.coords = cell.coords;
                ev.isLong = isLong;
            }

            if (cell.type == CellTypes.Barrier)
            {
                ref var ev = ref Events.global.Add<Event_CellBarrier_Clicked>();
                ev.coords = cell.coords;
                ev.isLong = isLong;
            } 
        }

        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public float TimeFromDown { get; set; }
    }
    
    public enum CellTypes
    {
        CanWalk,
        CanBuild,
        Empty,
        Barrier,
    }
}