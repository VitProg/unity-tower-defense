using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features._common.interfaces;
using td.utils;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.level.cells
{
    [Serializable]
    public struct Cell
    {
        public int2 coords;
        public int2 coordsAbs;
        public int index2D;
        
        public GameObject go;
        public CellMonoBehaviour mb;
        public CellTypes type;
        public ProtoPackedEntityWithWorld packedBuildingEntity;
        public string buildingId;
        public ProtoPackedEntityWithWorld packedShardEntity;
        
        public bool IsEmpty => coords is { x: 0, y: 0 };

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

        public List<IInputEventsHandler> inputEventsHandlers;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Cell FromCellEditor(CellMonoBehaviour cellMB)
        {
            return new Cell
            {
                mb = cellMB,
                go = cellMB.gameObject,
                coords = HexGridUtils.PositionToCell(cellMB.transform.position),
                type = cellMB.type,

                isKernel = cellMB.isKernel,
                kernelNumber = cellMB.kernelNumber,

                isSpawn = cellMB.isSpawn,
                spawnNumber = cellMB.spawnNumber,

                isSwitcher = cellMB.isSwitcher,
                isAutoNextSearching = cellMB.isAutoNextSearching,
                dirToNext = cellMB.directionToNext,
                dirToNextAlt = cellMB.directionToAltNext,
                
                inputEventsHandlers = new List<IInputEventsHandler> { cellMB },
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasBuilding() => packedBuildingEntity.Unpack(out _, out _);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasShard() => packedShardEntity.Unpack(out _, out _);
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return IsEmpty 
                ? "-"
                : $"{coords}:{(type == CellTypes.CanWalk ? "W:": "")}{(type == CellTypes.CanBuild ? "B:": "")}{(type == CellTypes.Barrier ? "X:": "")}{(HasBuilding() ? "T" : "")}{(HasShard() ? "S" : "")}";
        }
    }
}