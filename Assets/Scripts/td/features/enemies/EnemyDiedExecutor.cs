using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemies
{
    public class EnemyDiedExecutor : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<EnemyDiedCommand>> eventEnteties = Constants.Ecs.EventsWorldName;
        private readonly EcsCustomInject<LevelData> level = default;
        private readonly EcsCustomInject<UI> ui = default;
        private readonly EcsWorldInject world = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEnteties.Value)
            {
                ref var eventData = ref eventEnteties.Pools.Inc1.Get(eventEntity);

                if (!eventData.EnemyEntity.Unpack(world.Value, out var enemyEntity))
                {
                    continue;
                }

                var enemyStats = EntityUtils.GetComponent<SpawnEnemyCommand>(systems, enemyEntity);
                
                //todo тут можно запустиить анимацию смерти, эфекты, добавление очков и т.п.
                EntityUtils.AddComponent<RemoveGameObjectCommand>(systems, enemyEntity);

                level.Value.money += enemyStats.money;
                ui.Value.UpdateMoney(level.Value.money);
                    
                Debug.Log(">>> ENEMY IS DEAD!!");
            }
        }
    }
}