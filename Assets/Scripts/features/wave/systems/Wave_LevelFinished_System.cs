using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level.bus;

namespace td.features.wave.systems
{
    public class Wave_LevelFinished_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private Wave_State waveState;
        
        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_Wave_StateChanged>(OnWaveStateChanged);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Event_Wave_StateChanged>(OnWaveStateChanged);
        }
        
        // ----------------------------------------------------------------

        private void OnWaveStateChanged(ref Event_Wave_StateChanged ev)
        {
            if (!ev.waveNumber || !ev.enemiesCount) return;
            
            if (waveState.GetWaiting()) return;
            if (!waveState.AreAllWavesComplete()) return;
            if (waveState.GetEnemiesCount() > 0) return;
            
            //
            
            waveState.Clear();
            events.unique.GetOrAdd<Event_LevelFinished>();
        }
    }
}