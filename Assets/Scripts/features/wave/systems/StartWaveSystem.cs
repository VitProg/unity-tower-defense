using Leopotam.EcsLite;
using td.features.level;
using td.features.state;
using td.features.wave.bus;

namespace td.features.wave.systems
{
    public class StartWaveSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Wave_Service> waveService;

        public void Init(IEcsSystems systems)
        {
            events.Value.Unique.ListenTo<Command_Wave_Start>(StartWave);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Unique.RemoveListener<Command_Wave_Start>(StartWave);
        }
        
        //------------------------------------------//

        private void StartWave(ref Command_Wave_Start command)
        {
            waveService.Value.StartWave(command.waveNumber);
        }
    }
}