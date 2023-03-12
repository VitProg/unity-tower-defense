using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.flags;
using td.components.waves;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.waves
{
    public class LevelLoadedHandler : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<LevelLoadedEvent>> entities = Constants.Ecs.EventWorldName;

        public void Run(IEcsSystems systems)
        {
            var entity = EcsEventUtils.FirstEntity(entities);

            if (entity == null) return;
            
            Debug.Log("LevelLoadedHandler RUN...");
            
            GlobalEntityUtils.DelComponent<IsLoading>(systems);
            
            var countdown = levelData.Value.waveNumber <= 0
                ? levelData.Value.LevelConfig?.delayBeforeFirstWave
                : levelData.Value.LevelConfig?.delayBetweenWaves;
            
            EcsEventUtils.SendSingle(systems, new NextWaveCountdownTimer()
            {
                countdown = countdown ?? 0,
            });
            
            // EcsEventUtils.Send(systems, new StartWaveCommand()
            // {
            //     WaveNumber = 0,
            // });

            EcsEventUtils.CleanupEvent(systems, entities);
            
            Debug.Log("LevelLoadedHandler FIN");
        }
    }
}