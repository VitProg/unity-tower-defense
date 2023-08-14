using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.destroy;
using td.features.enemy.bus;
using td.features.eventBus;
using td.features.impactKernel;
using td.features.state;

namespace td.features.enemy.systems
{
    public class Enemy_ReachKernel_System : IProtoInitSystem, IProtoDestroySystem {
        [DI] private Enemy_Aspect aspect;
        [DI] private State state;
        [DI] private Enemy_Service enemyService;
        [DI] private Destroy_Service destroyService;
        [DI] private ImpactKernel_Service impactKernel;
        [DI] private EventBus events;
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Event_Enemy_ReachKernel>(OnEnemyReachKernel);
        }
        
        public void Destroy()
        {
            events.global.RemoveListener<Event_Enemy_ReachKernel>(OnEnemyReachKernel);
        }
        
        // ------------------------------------------------------------ //

        private void OnEnemyReachKernel(ref Event_Enemy_ReachKernel ev)
        {
            if (!enemyService.HasEnemy(ev.Entity, out var enemyEntity))
            {
                return;
            }
            ref var enemy = ref enemyService.GetEnemy(enemyEntity);
                
            //todo тут можно запустиить анимацию атаки на ядро, пропадания врага, эфекты, вычитание жизней ядра и т.п.
            enemyService.SetIsDead(enemyEntity, true);
            destroyService.MarkAsRemoved(aspect.World().PackEntityWithWorld(enemyEntity));
            impactKernel.TakeDamage(enemy.damage);

            state.SetEnemiesCount(state.GetEnemiesCount() - 1);
        }
    }
}