using Leopotam.EcsLite;

namespace td.features.infoPanel.bus
{
    public struct Command_HideTowerInfo : IEventGlobal
    {
        public EcsPackedEntityWithWorld towerEntity;
    }
}