using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.eventBus;
using td.features.gameStatus.bus;
using td.features.state;

namespace td.features.gameStatus.systems
{
    public class GameSimilationStatusSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        // [DI] private Common_Service common;
        [DI] private State state;
        
        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Command_StartGame>(OnGameStarted);
            events.unique.ListenTo<Command_StopGameSimulation>(StopGameSimulation);
            events.unique.ListenTo<Command_ResumeGameSimulation>(ResumeGameSimulation);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Command_StartGame>(OnGameStarted);
            events.unique.RemoveListener<Command_StopGameSimulation>(StopGameSimulation);
            events.unique.RemoveListener<Command_ResumeGameSimulation>(ResumeGameSimulation);
        }
        
        // ---------------------------------------------------------------- //
        
        private void ResumeGameSimulation(ref Command_ResumeGameSimulation _)
        {
            state.SetGameSimulationEnabled(true);
            // common.SetGroupSystemState(Constants.EcsSystemGroups.GameSimulation, true);
        }

        private void StopGameSimulation(ref Command_StopGameSimulation _)
        {
            state.SetGameSimulationEnabled(false);
            // common.SetGroupSystemState(Constants.EcsSystemGroups.GameSimulation, false);
        }

        private void OnGameStarted(ref Command_StartGame item)
        {
            state.SetGameSimulationEnabled(true);
            // common.SetGroupSystemState(Constants.EcsSystemGroups.GameSimulation, true);
            state.SetGameSpeed(1f);
        }
    }
}