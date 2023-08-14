using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.infoPanel.bus;
using td.features.state;
using td.utils.ecs;
using UnityEngine;

namespace td.features.infoPanel
{
    public class InfoPanel_Module : IProtoModuleWithEvents, IProtoModuleWithStateEx
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            
            systems
                .AddSystem(new InfoPanel_System())
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

        public IStateExtension StateEx() => new InfoPanel_StateExtension();

        public Type[] Events() => Ev.E<
            Command_HideTowerInfo,
            Command_ShowTowerInfo
        >();
    }
}