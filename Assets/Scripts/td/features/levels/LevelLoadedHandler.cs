using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.features.waves;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.levels
{
    public class LevelLoadedHandler : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<LevelLoadedEvent>> entities = Constants.Ecs.EventsWorldName;
        private readonly EcsCustomInject<UI> ui = default;

        public void Run(IEcsSystems systems)
        {
            var entity = EcsEventUtils.FirstEntity(entities);

            if (entity == null) return;
            
            Debug.Log("LevelLoadedHandler RUN...");
            
            GlobalEntityUtils.DelComponent<IsLoading>(systems);
            
            ui.Value.UpdateLives((int)levelData.Value.Lives);
            ui.Value.UpdateWave(0, 0);
            ui.Value.UpdateMoney(levelData.Value.money);
            
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