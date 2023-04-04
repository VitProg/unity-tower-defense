using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.levels;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.waves
{
    public class IncreaseWaveEcecutor: IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<IncreaseWaveCommand>> entities = Constants.Ecs.EventsWorldName;
        private readonly EcsCustomInject<UI> ui = default;

        public void Run(IEcsSystems systems)
        {
            var eventsWorld = systems.GetWorld(Constants.Ecs.EventsWorldName);

            if (EcsEventUtils.FirstEntity(entities) == null) return;
            
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
            
            ui.Value.UpdateWave(waveNumber + 1, waveCount);

            EcsEventUtils.CleanupEvent(eventsWorld, entities);
            
            Debug.Log("IncreaseWaveHanndler FIN");
        }
    }
}