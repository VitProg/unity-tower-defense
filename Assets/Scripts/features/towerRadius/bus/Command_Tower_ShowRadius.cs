using Leopotam.EcsLite;

namespace td.features.towerRadius.bus
{
    public struct Command_Tower_ShowRadius : IEventGlobal
    {
        public EcsPackedEntityWithWorld towerEntity;
    }
}