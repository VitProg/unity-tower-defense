using Leopotam.EcsLite;
using td.features._common;
using td.features.gameStatus.bus;
using td.features.state;

namespace td.features.gameStatus.systems
{
    public class GameSimilationStatusSystem : IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<IState> state;
        
        public void Run(IEcsSystems systems)
        {
            events.Value.Unique.ListenTo<Command_StartGame>(OnGameStarted);
            events.Value.Unique.ListenTo<Command_StopGameSimulation>(StopGameSimulation);
            events.Value.Unique.ListenTo<Command_ResumeGameSimulation>(ResumeGameSimulation);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Unique.RemoveListener<Command_StartGame>(OnGameStarted);
            events.Value.Unique.RemoveListener<Command_StopGameSimulation>(StopGameSimulation);
            events.Value.Unique.RemoveListener<Command_ResumeGameSimulation>(ResumeGameSimulation);
        }
        
        // ---------------------------------------------------------------- //
        

        private void ResumeGameSimulation(ref Command_ResumeGameSimulation _)
        {
            common.Value.SetGroupSystemState(Constants.EcsSystemGroups.GameSimulation, true);
        }

        private void StopGameSimulation(ref Command_StopGameSimulation _)
        {
            common.Value.SetGroupSystemState(Constants.EcsSystemGroups.GameSimulation, false);
        }

        private void OnGameStarted(ref Command_StartGame item)
        {
            common.Value.SetGroupSystemState(Constants.EcsSystemGroups.GameSimulation, true);
            state.Value.GameSpeed = 1f;
        }
    }
}