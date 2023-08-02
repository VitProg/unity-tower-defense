using Leopotam.EcsLite;

namespace td.features.infoPanel.bus
{
    public struct Command_ShowTowerInfo : IEventGlobal
    {
        public EcsPackedEntityWithWorld towerEntity;
    }
}