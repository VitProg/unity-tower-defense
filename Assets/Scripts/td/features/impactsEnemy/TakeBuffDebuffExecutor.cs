using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.enemies;
using td.utils.ecs;

namespace td.features.impactsEnemy
{
    public class TakeBuffDebuffExecutor : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<TakeBuffDebuffCommand>> eventEntities = Constants.Ecs.EventsWorldName;
        private readonly EcsWorldInject world = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                var buff = eventEntities.Pools.Inc1.Get(eventEntity);

                if (!buff.TargetEntity.Unpack(world.Value, out var enemyEntity) ||
                    !EntityUtils.HasComponent<SpawnEnemyCommand>(systems, enemyEntity))
                {
                    continue;
                }
                
                ref var enemyStat = ref EntityUtils.GetComponent<SpawnEnemyCommand>(systems, enemyEntity);
                
                // ToDo
            }
        }
    }
}