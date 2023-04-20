using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.levels;
using td.services;
using td.states;
using td.utils.ecs;
using UnityEngine;

namespace td.features.waves
{
    public class IncreaseWaveEcecutor: IEcsRunSystem
    {
        [EcsInject] private LevelState levelState;
        
        private readonly EcsFilterInject<Inc<IncreaseWaveOuterCommand>> eventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            if (eventEntities.Value.GetEntitiesCount() == 0) return;
            systems.CleanupOuter(eventEntities);

            // Debug.Log("IncreaseWaveHanndler RUN...");
            
            var waveNumber = levelState.WaveNumber;
            var waveCount = levelState.WaveCount;

            if (levelState.IsLastWave)
            {
                systems.SendOuter<LevelFinishedOuterEvent>();
                return;
            }

            waveNumber++;

            // systems.SendOuter(new WaveChangedOuterEvent()
            // {
                // WaveNumber = waveNumber,
            // });
            systems.SendOuter(new StartWaveOuterCommand()
            {
                WaveNumber = waveNumber,
            });

            levelState.WaveNumber = waveNumber;
            
            // systems.CleanupOuter(eventEntities);
            
            // Debug.Log("IncreaseWaveHanndler FIN");
        }
    }
}