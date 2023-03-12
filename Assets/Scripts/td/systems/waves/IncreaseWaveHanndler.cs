using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.waves;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.waves
{
    public class IncreaseWaveHanndler: IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<IncreaseWaveCommand>> entities = Constants.Ecs.EventWorldName;
        
        public void Run(IEcsSystems systems)
        {
            var eventsWorld = systems.GetWorld(Constants.Ecs.EventWorldName);

            if (EcsEventUtils.Single(entities) == null) return;
            
            Debug.Log("IncreaseWaveHanndler RUN...");
            
            ref var waveNumber = ref levelData.Value.waveNumber;
            var waveCount = levelData.Value.WavesCount;

            if (waveNumber + 1 > waveCount - 1)
            {
                EcsEventUtils.Send<LevelFinishedEvent>(eventsWorld);
                return;
            }

            waveNumber++;

            EcsEventUtils.Send(eventsWorld, new WaveChangedEvent()
            {
                WaveNumber = waveNumber,
            });
            EcsEventUtils.Send(eventsWorld, new StartWaveCommand()
            {
                WaveNumber = waveNumber,
            });

            EcsEventUtils.CleanupEvent(eventsWorld, entities);
            
            Debug.Log("IncreaseWaveHanndler FIN");
        }
    }
}