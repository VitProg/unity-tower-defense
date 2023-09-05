using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level;
using td.features.level.bus;
using td.features.state;

namespace td.features.wave.systems
{
    public class Wave_InitSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private State state;
        [DI] private Level_State levelState;
        [DI] private Wave_State waveState;

        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_LevelLoaded>(OnLevelLoaded);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Event_LevelLoaded>(OnLevelLoaded);
        }
        
        // ----------------------------------------------------------------

        private void OnLevelLoaded(ref Event_LevelLoaded ev)
        {
            ref var cfg = ref levelState.GetLevelConfig();
            
            if (cfg.IsEmpty()) throw new Exception("Level Config is empty!");
            
            waveState.Clear();
            waveState.SetWaiting(true);
            waveState.SetNextWaveCountdown(cfg.delayBeforeFirstWave);
            waveState.SetEnemiesCount(0);
            waveState.SetWaveNumber(0);
            waveState.SetWaveCount(cfg.waves.Length);
        }
    }
}