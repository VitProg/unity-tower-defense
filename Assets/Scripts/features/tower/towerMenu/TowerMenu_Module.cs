using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.tower.towerMenu.bus;

namespace td.features.tower.towerMenu
{
    public class TowerMenu_Module : IProtoModuleWithEvents
    {
        public void Init(IProtoSystems systems)
        {
            // todo
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
            Command_ShowTowerMenu,
            Command_HideTowerMenu
        >();
    }
}