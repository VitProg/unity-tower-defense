using td.features.eventBus.types;
using Unity.Mathematics;

namespace td.features.building.buildingShop.bus
{
    public struct Command_BuyBuilding : IGlobalEvent
    {
        public string buildingId;
        public int2 cellCoords;
        public uint price;
        public uint buildTime;
    }
}