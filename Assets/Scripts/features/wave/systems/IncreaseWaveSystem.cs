using Leopotam.EcsLite;
using td.features.level.bus;
using td.features.state;
using td.features.wave.bus;

namespace td.features.wave.systems
{
    public class IncreaseWaveSystem: IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;
        
        public void Init(IEcsSystems systems)
        {
            events.Value.Unique.ListenTo<Command_Wave_Increase>(IncreaseWave);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Unique.RemoveListener<Command_Wave_Increase>(IncreaseWave);
        }
        
        //------------------------------------------//
        
        private void IncreaseWave(ref Command_Wave_Increase _)
        {
            var waveNumber = state.Value.WaveNumber;
            var waveCount = state.Value.WaveCount;

            if (waveNumber + 1 > waveCount)
            {
                events.Value.Unique.Add<Event_LevelFinished>();
            }
            else
            {
                waveNumber++;
                events.Value.Unique.Add<Command_Wave_Start>().waveNumber = waveNumber;
                state.Value.WaveNumber = waveNumber;
            }
        }

    }
}