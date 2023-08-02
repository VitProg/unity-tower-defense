using Leopotam.EcsLite;

namespace td.features.towerRadius.bus
{
    public struct Command_Tower_HideRadius : IEventGlobal
    {
        public EcsPackedEntityWithWorld towerEntity;
    }
}