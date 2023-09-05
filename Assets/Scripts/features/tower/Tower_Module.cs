using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.tower.bus;
using td.features.tower.systems;
using td.features.tower.towerMenu;
using td.features.tower.towerRadius;
using td.utils.ecs;

namespace td.features.tower
{
    public class Tower_Module : IProtoModuleWithEvents
    {
        private readonly Func<float> getDeltaTime;
        
        public Tower_Module(Func<float> getDeltaTime)
        {
            // Debug.Log($"{GetType().Name} Init");
            this.getDeltaTime = getDeltaTime;
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new Tower_FindTarget_System(1/5f, 0.1f, getDeltaTime))
                .AddSystem(new Tower_InitOnLevelStart_System())
                .AddSystem(new Tower_BuyHandler_System())
                //
                .AddSystem(new ShardTower_Fire_System())
                .AddSystem(new ShardTower_ShardChangesListener_System())
                //
                .AddService(new Tower_Converter(), true)
                .AddService(new Tower_Service(), true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new Tower_Aspect(),
            };
        }

        public IProtoModule[] Modules()
        {
            return new IProtoModule[]
            {
                new TowerRadius_Module(getDeltaTime),
                new TowerMenu_Module(),
            };
        }

        public Type[] Events() => Ev.E<
            Event_Tower_Clicked,
            Event_Tower_Hovered,
            Event_Tower_UnHovered,
            Event_Tower_Created
        >();
    }
}