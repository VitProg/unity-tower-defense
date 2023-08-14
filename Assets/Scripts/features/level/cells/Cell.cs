using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.common;
using td.monoBehaviours;
using td.utils;
using UnityEngine;

namespace td.features.level.cells
{
    [Serializable]
    public struct Cell
    {
        public Int2 coords;
        public GameObject go;
        public CellTypes type;
        public ProtoPackedEntityWithWorld? packedBuildingEntity;
        public ProtoPackedEntityWithWorld? packedShardEntity;

        public bool IsEmpty => coords.IsZero;

        public bool isKernel;
        public byte kernelNumber;
        
        public bool isSpawn;
        public byte spawnNumber;
        
        public bool isSwitcher;
        public bool isAutoNextSearching;
        public HexDirections dirToNext;
        public HexDirections dirToNextAlt;
        public HexDirections dirToPrev;

        public ushort distanceFromSpawn;
        public ushort distanceToKernel;
        public bool isPathAnalyzed;

        public bool HasNextDir => dirToNext != HexDirections.NONE; 
        public bool HasNextAltDir => dirToNextAlt != HexDirections.NONE; 

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Cell FromCellEditor(CellEditorMB cell)
        {
            return new Cell()
            {
                go = cell.gameObject,
                coords = HexGridUtils.PositionToCell(cell.transform.position),
                type = cell.type,

                isKernel = cell.isKernel,
                kernelNumber = cell.kernelNumber,

                isSpawn = cell.isSpawn,
                spawnNumber = cell.spawnNumber,

                isSwitcher = cell.isSwitcher,
                isAutoNextSearching = cell.isAutoNextSearching,
                dirToNext = cell.directionToNext,
                dirToNextAlt = cell.directionToAltNext,
            };
        }
        
        public override string ToString()
        {
            return IsEmpty 
                ? "-"
                : $"{coords}:{(type == CellTypes.CanWalk ? "W:": "")}{(type == CellTypes.CanBuild ? "B:": "")}{(type == CellTypes.Barrier ? "X:": "")}{(packedBuildingEntity.HasValue ? "T" : "")}";
        }
    }
}