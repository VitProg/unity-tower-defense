using System;
using Leopotam.EcsProto;
using td.features.tower.systems;
using td.features.tower.towerMenu;
using td.features.tower.towerRadius;
using td.utils.ecs;

namespace td.features.tower
{
    public class Tower_Module : IProtoModule
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
                .AddSystem(new TowerFindTargetSystem(1/5f, 0.1f, getDeltaTime))
                .AddSystem(new ShardTowerFireSystem())
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
                new TowerRadius_Module(),
                new TowerMenu_Module(),
            };
        }
    }
}