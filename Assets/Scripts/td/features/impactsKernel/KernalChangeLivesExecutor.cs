using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.services;
using td.utils;
using UnityEditor;

namespace td.features.impactsKernel
{
    public class KernalChangeLivesExecutor : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<KernalDamageCommand>> damageCommands = Constants.Ecs.EventsWorldName;
        private readonly EcsFilterInject<Inc<KernelHealCommand>> healCommands = Constants.Ecs.EventsWorldName;
        private readonly EcsWorldInject world = default;
        private readonly EcsSharedInject<SharedData> shared = default;
        private readonly EcsCustomInject<LevelData> level = default;
        private readonly EcsCustomInject<UI> ui = default;

        public void Run(IEcsSystems systems)
        {
            var lives = level.Value.Lives;
            
            foreach (var damage in damageCommands.Value)
            {
                lives -= damageCommands.Pools.Inc1.Get(damage).damage;
            }
            
            foreach (var heal in healCommands.Value)
            {
                lives -= healCommands.Pools.Inc1.Get(heal).damage;
            }

            if (!FloatUtils.IsEquals(lives, level.Value.Lives))
            {
                level.Value.Lives = lives;
                ui.Value.UpdateLives((int)lives);
            }
        }
    }
}