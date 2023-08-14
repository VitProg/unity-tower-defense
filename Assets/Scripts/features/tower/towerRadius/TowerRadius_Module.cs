using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.tower.towerRadius.bus;
using td.features.towerRadius;
using td.features.towerRadius.bus;
using td.utils.ecs;

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
            return null;/*new IProtoAspect[]
            {
                new Tower_Aspect(),
            };*/
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