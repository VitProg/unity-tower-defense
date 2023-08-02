using Leopotam.EcsLite;
using td.features.state;
using td.features.wave.bus;

namespace td.features.wave.systems
{
    public class SpawnSequenceFinishedSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;
        
        public void Init(IEcsSystems systems)
        {
            events.Value.Unique.ListenTo<Event_SpawnSequence_Finished>(OnSpawnSequenceFinished);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Unique.RemoveListener<Event_SpawnSequence_Finished>(OnSpawnSequenceFinished);
        }
        
        //------------------------------------------//
        
        private void OnSpawnSequenceFinished(ref Event_SpawnSequence_Finished @event) {
            state.Value.ActiveSpawnCount--;
            if (state.Value.ActiveSpawnCount <= 0)
                events.Value.Unique.Add<Wait_AllEnemiesAreOver>();
        }
    }
}