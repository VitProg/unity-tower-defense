using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.flags;
using td.features.enemies.components;
using td.services;
using td.utils;
using td.utils.ecs;

namespace td.features.enemies.systems
{
    public class EnemyDiedExecutor : IEcsRunSystem
    {
        [Inject] private LevelState levelState;
        [InjectWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<EnemyDiedCommand, Enemy>, Exc<IsDestroyed>> enteties = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var enemyEntity in enteties.Value)
            {
                ref var enemy = ref enteties.Pools.Inc2.Get(enemyEntity);

                //todo тут можно запустиить анимацию смерти, эфекты, добавление очков и т.п.
                world.GetComponent<IsDisabled>(enemyEntity);
                world.GetComponent<RemoveGameObjectCommand>(enemyEntity);

                levelState.Money += enemy.money;

                // Debug.Log(">>> ENEMY IS DEAD!!");
            }

            if (enteties.Value.GetEntitiesCount() > 0)
            {
                levelState.EnemiesCount = EnemyUtils.GetEnemiesCount(world);
            }
        }
    }
}