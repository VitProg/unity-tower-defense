using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.enemies;
using td.utils.ecs;

namespace td.features.impactsEnemy
{
    public class TakeBuffDebuffExecutor : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<TakeDebuffOuterCommand>> eventEntities = Constants.Worlds.Outer;
        [EcsWorld] private EcsWorld world;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                var buff = eventEntities.Pools.Inc1.Get(eventEntity);

                if (!buff.TargetEntity.Unpack(world, out var enemyEntity) ||
                    !world.HasComponent<SpawnEnemyOuterCommand>(enemyEntity))
                {
                    continue;
                }
                
                ref var enemyState = ref world.GetComponent<EnemyState>(enemyEntity);
                
                // ToDo
            }
        }
    }
}