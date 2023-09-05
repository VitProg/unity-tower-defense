using System;
using Leopotam.EcsProto;
using td.features.state.interfaces;

namespace td.features.enemy.enemyPath {
    public class EnemyPath_Module : IProtoModuleWithStateEx {
        private readonly Func<float> getDeltaTime;
        
        public EnemyPath_Module(Func<float> getDeltaTime) {
            this.getDeltaTime = getDeltaTime;
        }

        public void Init(IProtoSystems systems) {
            systems
                .AddSystem(new EnemyPath_CacheRoutes_System())
                .AddSystem(new Enemy_CalcDistanceToKernel_System(1 / 15f, 0f, getDeltaTime))
                //
                .AddService(new EnemyPath_Service())
                ;
        }

        public IProtoAspect[] Aspects() {
            return null;
        }

        public IProtoModule[] Modules() {
            return null;
        }

        public IStateExtension StateEx() => new EnemyPath_State();
    }
}
