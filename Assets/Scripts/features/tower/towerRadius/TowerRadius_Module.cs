using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.tower.towerRadius.bus;

namespace td.features.tower.towerRadius
{
    public class TowerRadius_Module : IProtoModuleWithEvents
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new TowerRadius_System())
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public Type[] Events() => Ev.E<
            Command_Tower_HideRadius,
            Command_Tower_ShowRadius
        >();
    }
}