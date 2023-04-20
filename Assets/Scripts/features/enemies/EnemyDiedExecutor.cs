using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.services;
using td.states;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies
{
    public class EnemyDiedExecutor : IEcsRunSystem
    {
        [EcsInject] private LevelState levelState;
        [EcsWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<EnemyDiedCommand, Enemy>> enteties = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var enemyEntity in enteties.Value)
            {
                ref var enemy = ref enteties.Pools.Inc2.Get(enemyEntity);

                //todo тут можно запустиить анимацию смерти, эфекты, добавление очков и т.п.
                world.AddComponent<RemoveGameObjectCommand>(enemyEntity);

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