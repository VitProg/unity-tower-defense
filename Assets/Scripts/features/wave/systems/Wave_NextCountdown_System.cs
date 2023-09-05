using System;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level;
using td.features.state;
using td.utils.ecs;

namespace td.features.wave.systems
{
    public class Wave_NextCountdown_System : ProtoIntervalableRunSystem
    {
        [DI] private EventBus events;
        [DI] private State state;
        [DI] private Wave_State waveState;
        [DI] private Level_State levelState;
        
        public override void IntervalRun(float deltaTime)
        {
            if (state.GetGameSpeed() == 0 || !state.GetSimulationEnabled()) return;
            if (waveState.GetWaiting()) return;
            if (waveState.IsWaveActive()) return;
            if (waveState.GetActiveSpawnersCount() > 0) return;
            if (waveState.GetEnemiesCount() > 0) return;
            
            if (waveState.GetNextWaveCountdown() > 0f) {
                waveState.ReduceNextWaveCountdown(state.GetGameSpeed() * deltaTime);
                return;
            }
            
            ref var cfg = ref levelState.GetLevelConfig();
            
            waveState.IncreaseWaveNumber();
            waveState.SetNextWaveCountdown(cfg.delayBetweenWaves);
            
            //

            waveState.ClearSpawners();
                
            // todo
            var wave = cfg.waves[waveState.GetWaveNumber() - 1];
            for (var index = 0; index < wave.spawns.Length; index++)
            {
                ref var spawn = ref wave.spawns[index];
                waveState.AddSpawner(ref spawn);
            }
        }

        public Wave_NextCountdown_System(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}