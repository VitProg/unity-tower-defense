using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.tower.towerRadius.bus;

namespace td.features.tower.towerRadius
{
    public class TowerRadius_Module : IProtoModuleWithEvents
    {
        private readonly Func<float> getDeltaTime;
        
        public TowerRadius_Module(Func<float> getDeltaTime)
        {
            // Debug.Log($"{GetType().Name} Init");
            this.getDeltaTime = getDeltaTime;
        }

        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new TowerRadius_Visible_System(1/15f, 0f, getDeltaTime))
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

        public Type[] Events() => Ev.E<Command_ShowTowerRadius>();
    }
}