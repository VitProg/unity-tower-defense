using Leopotam.EcsLite;
using td.common.cells.interfaces;

namespace td.common.cells.hex
{
    public class HexCellCanBuild : HexCell, ICellCanBuild
    {
        public EcsPackedEntity? BuildingPackedEntity { get; set; }
        public bool HasBuilding => BuildingPackedEntity != null;
    }
}