using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level;
using td.features.state;
using td.features.wave.bus;

namespace td.features.wave.systems
{
    public class StartWaveSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private LevelMap levelMap;
        [DI] private EventBus events;
        [DI] private Wave_Service waveService;

        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Command_Wave_Start>(StartWave);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Command_Wave_Start>(StartWave);
        }
        
        //------------------------------------------//

        private void StartWave(ref Command_Wave_Start command)
        {
            waveService.StartWave(command.waveNumber);
        }
    }
}