using Leopotam.EcsLite;
using NaughtyAttributes;
using td.services;
using td.common;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.monoBehaviours
{
#if UNITY_EDITOR
    [SelectionBase] // Если вы кликнете на внутреннюю запчасть префаба, то выделится именно этот объект
#endif
    public class Cell : MonoBehaviour
    {
        #region properties

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
        
        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)]
        public HexDirections directionToPrev = HexDirections.NONE;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)] [DisableIf("isSpawn")]
        public bool isKernel;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)] [DisableIf("isKernel")]
        public bool isSpawn;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)] [EnableIf("isKernel")]
        public uint kernelNumber = 0;

        [BoxGroup("Main Parapeters")] [ShowIf("type", CellTypes.CanWalk)] [EnableIf("isSpawn")]
        public uint spawnNumber = 0;

        [Inject] private LevelMap levelMap;
        [Inject] private LevelState levelState;

        [ShowNativeProperty]
        public bool HasDirectionToNext => type == CellTypes.CanWalk && directionToNext != HexDirections.NONE &&
                               directionToNext != HexDirections.NONE;

        [ShowNativeProperty]
        public bool HasAltSirectionToNext => type == CellTypes.CanWalk && isSwitcher && directionToAltNext != HexDirections.NONE &&
                                  directionToAltNext != HexDirections.NONE;

        public Int2? NextCoords => HasDirectionToNext ? HexGridUtils.GetNeighborsCoords(Coords, directionToNext) : null;
        public Int2? AltNextCoords => HasAltSirectionToNext ? HexGridUtils.GetNeighborsCoords(Coords, directionToAltNext) : null;

        // todo
        public readonly EcsPackedEntity?[] buildings = new EcsPackedEntity?[3];

        [ShowNativeProperty]
        public bool HasBuilding => buildings[0] != null || buildings[1] != null || buildings[2] != null;

        private Int2? coords;
        public Int2 Coords
        {
            get
            {
                coords ??= HexGridUtils.PositionToCell(this.transform.position);
                return coords.Value;
            }
        }

        public Vector2 Position => gameObject.transform.position;

        [ReadOnly] public uint distanceFromSpawn = 0;
        [ReadOnly] public uint distanceToKernel = 0;
        [ReadOnly] public bool isPathAnalyzed = false;

        public Int2? GetRandomNextCoords() =>
            isSwitcher && HasDirectionToNext && HasAltSirectionToNext
                ? Random.value > .5 ? AltNextCoords : NextCoords
                : NextCoords; 
        
        public HexDirections GetRandomNextDirection() =>
            isSwitcher && HasDirectionToNext && HasAltSirectionToNext
                ? Random.value > .5 ? directionToAltNext : directionToNext
                : directionToNext;

        #endregion

        private void Update()
        {
            if (transform.hasChanged)
            {
                coords = HexGridUtils.PositionToCell(this.transform.position);
            }

            if (levelMap == null && DI.IsReady)
            {
                DI.Resolve(this);
            }
        }
        
        private void Start()
        {
            // DI.Resolve(this);
            
            coords = HexGridUtils.PositionToCell(this.transform.position);

#if UNITY_EDITOR
            // transform.GetComponent<CellDebug>().cell = this;
#else
            Destroy(transform.GetComponent<CellDebug>().gameObject);
#endif
        }
    }
    
    public enum CellTypes
    {
        CanWalk,
        CanBuild,
        Empty,
        Barrier,
    }
}