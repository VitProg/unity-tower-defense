using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.gameStatus.bus;
using td.features.gameStatus.systems;

namespace td.features.gameStatus
{
    public class GameStatus_Module : IProtoModuleWithEvents
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            
            systems
                .AddSystem(new GameSimilationStatusSystem())
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
            Command_ResumeGameSimulation,
            Command_StartGame,
            Command_StopGameSimulation,
            Event_YouDied
        >();
    }
}