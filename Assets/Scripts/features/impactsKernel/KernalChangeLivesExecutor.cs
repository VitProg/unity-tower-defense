using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.states;
using td.utils;
using td.utils.ecs;

namespace td.features.impactsKernel
{
    public class KernalChangeLivesExecutor : IEcsRunSystem
    {
        [EcsInject] private LevelState levelState;
        
        private readonly EcsFilterInject<Inc<KernalDamageOuterCommand>> damageCommands = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<KernelHealOuterCommand>> healCommands = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            var lives = levelState.Lives;
            
            foreach (var damage in damageCommands.Value)
            {
                lives -= damageCommands.Pools.Inc1.Get(damage).damage;
            }
            
            foreach (var heal in healCommands.Value)
            {
                lives -= healCommands.Pools.Inc1.Get(heal).damage;
            }
            
            if (!FloatUtils.IsEquals(lives, levelState.Lives))
            {
                levelState.Lives = lives;
            }
        }
    }
}