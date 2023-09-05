using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy.bus;
using td.features.eventBus;

namespace td.features.wave.systems
{
    public class Wave_EnemiesDied_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private Wave_State waveState;
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Event_Enemy_Died>(OnEnemyDied);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Event_Enemy_Died>(OnEnemyDied);
        }
        
        // ----------------------------------------------------------------

        private void OnEnemyDied(ref Event_Enemy_Died ev)
        {
            waveState.ReduceEnemiesCount();
        }
    }
}