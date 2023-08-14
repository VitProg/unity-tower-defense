using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level.bus;
using td.features.state;
using td.features.wave.bus;

namespace td.features.wave.systems
{
    public class IncreaseWaveSystem: IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private EventBus events;
        
        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Command_Wave_Increase>(IncreaseWave);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Command_Wave_Increase>(IncreaseWave);
        }
        
        //------------------------------------------//
        
        private void IncreaseWave(ref Command_Wave_Increase _)
        {
            var waveNumber = state.GetWaveNumber();
            var waveCount = state.GetWaveCount();

            if (waveNumber + 1 > waveCount)
            {
                events.unique.GetOrAdd<Event_LevelFinished>();
            }
            else
            {
                waveNumber++;
                events.unique.GetOrAdd<Command_Wave_Start>().waveNumber = waveNumber;
                state.SetWaveNumber(waveNumber);
            }
        }

    }
}