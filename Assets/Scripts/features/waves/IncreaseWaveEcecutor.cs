using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.levels;
using td.features.state;
using td.utils.ecs;

namespace td.features.waves
{
    public class IncreaseWaveEcecutor: IEcsRunSystem
    {
        [Inject] private State state;
        
        private readonly EcsFilterInject<Inc<IncreaseWaveOuterCommand>> eventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            if (eventEntities.Value.GetEntitiesCount() == 0) return;
            systems.CleanupOuter(eventEntities);

            // Debug.Log("IncreaseWaveHanndler RUN...");
            
            var waveNumber = state.WaveNumber;
            var waveCount = state.WaveCount;

            if (waveNumber + 1 >= waveCount)
            {
                systems.Outer<LevelFinishedOuterEvent>();
                return;
            }

            waveNumber++;

            // systems.Outer(new WaveChangedOuterEvent()
            // {
                // WaveNumber = waveNumber,
            // });
            systems.OuterSingle<StartWaveOuterCommand>().WaveNumber = waveNumber;

            state.WaveNumber = waveNumber;
            
            // systems.DelOuter(eventEntities);
            
            // Debug.Log("IncreaseWaveHanndler FIN");
        }
    }
}