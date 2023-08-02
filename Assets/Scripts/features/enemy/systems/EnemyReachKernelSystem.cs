using Leopotam.EcsLite;
using td.features._common;
using td.features.enemy.bus;
using td.features.impactKernel;
using td.features.state;

namespace td.features.enemy.systems
{
    public class EnemyReachKernelSystem : IEcsInitSystem, IEcsDestroySystem {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<ImpactKernel_Service> impactKernel;
        private readonly EcsInject<IEventBus> events;
        
        public void Init(IEcsSystems systems)
        {
            events.Value.Entity.ListenTo<Event_Enemy_ReachKernel>(OnEnemyReachKernel);
        }
        
        public void Destroy(IEcsSystems systems)
        {
            events.Value.Entity.RemoveListener<Event_Enemy_ReachKernel>(OnEnemyReachKernel);
        }
        
        // ------------------------------------------------------------ //

        private void OnEnemyReachKernel(EcsPackedEntityWithWorld packedEntity, ref Event_Enemy_ReachKernel @event)
        {
            if (!enemyService.Value.HasEnemy(packedEntity, out var enemyEntity))
            {
                return;
            }
            ref var enemy = ref enemyService.Value.GetEnemy(enemyEntity);
                
            //todo тут можно запустиить анимацию атаки на ядро, пропадания врага, эфекты, вычитание жизней ядра и т.п.
            enemyService.Value.SetIsDead(enemyEntity, true);
            common.Value.SafeDelete(enemyEntity);
            impactKernel.Value.TakeDamage(enemy.damage);

            state.Value.EnemiesCount--;
        }
    }
}