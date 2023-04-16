using System;
using Leopotam.EcsLite;
using td.common.cells.interfaces;
using UnityEngine;

namespace td.common.cells
{
    [Serializable]
    public class CellCanBuild: ICell, ICellCanBuild
    {
        public Int2 Coordinates { get; set; }
        public EcsPackedEntity? BuildingPackedEntity { get; set; }
        public bool HasBuilding => BuildingPackedEntity != null;
    }
}