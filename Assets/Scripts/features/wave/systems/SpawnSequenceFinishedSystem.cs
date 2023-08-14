using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.state;
using td.features.wave.bus;

namespace td.features.wave.systems
{
    public class SpawnSequenceFinishedSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private EventBus events;
        
        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_SpawnSequence_Finished>(OnSpawnSequenceFinished);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Event_SpawnSequence_Finished>(OnSpawnSequenceFinished);
        }
        
        //------------------------------------------//
        
        private void OnSpawnSequenceFinished(ref Event_SpawnSequence_Finished @event) {
            state.SetActiveSpawnCount(state.GetActiveSpawnCount() - 1);
            if (state.GetActiveSpawnCount() <= 0)
                events.unique.GetOrAdd<Wait_AllEnemiesAreOver>();
        }
    }
}