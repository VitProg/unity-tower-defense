using Leopotam.EcsLite;
using td.features.state;

namespace td.features.enemy.systems
{
    public class EnemyFixAnimationSpeedSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Enemy_Pools> pools;
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Enemy_Service> enemyService;

        public void Init(IEcsSystems systems)
        {
            events.Value.Unique.ListenTo<Event_StateChanged>(OnStateChanged);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Unique.RemoveListener<Event_StateChanged>(OnStateChanged);
        }
        
        // ---------------------------------------------------------------- //

        private void OnStateChanged(ref Event_StateChanged item)
        {
            if (!item.gameSpeed.HasValue) return;
            
            var gameSpeed = state.Value.GameSpeed;

            var count = pools.Value.livingEnemiesFilter.Value.GetEntitiesCount();
            var arr = pools.Value.livingEnemiesFilter.Value.GetRawEntities();
            for (var index = 0; index < count; index++)
            {
                var enemyMb = enemyService.Value.GetEnemyMB(arr[index]);
                if (enemyMb != null && enemyMb.animator != null && enemyMb.baseAnimationSpeed > 0f)
                {
                    enemyMb.animator.speed = enemyMb.baseAnimationSpeed * gameSpeed;
                }
            }
        }
    }
}