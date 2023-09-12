using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.state.interfaces;
using td.features.wave.bus;
using td.features.wave.systems;

namespace td.features.wave
{
    public class Wave_Module : IProtoModuleWithEvents, IProtoModuleWithStateEx
    {
        private readonly Func<float> getDeltaTime;
        
        public Wave_Module(Func<float> getDeltaTime)
        {
            this.getDeltaTime = getDeltaTime;
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new Wave_InitSystem())
                .AddSystem(new Wave_WaitPlayerAction_System())
                .AddSystem(new Wave_NextCountdown_System(1/30f, 0.033f, getDeltaTime))
                .AddSystem(new Wave_SpawnSequence_System(1/30f, 0.066f, getDeltaTime))
                .AddSystem(new Wave_LevelFinished_System())
                .AddSystem(new Wave_EnemiesDied_System())
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

        public IStateExtension StateEx() => new Wave_State();

        public Type[] Events() => Ev.E<
            Command_StartNextWave
        >();
    }
}