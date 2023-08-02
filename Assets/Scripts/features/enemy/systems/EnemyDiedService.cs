using Leopotam.EcsLite;
using td.features._common;
using td.features.enemy.bus;
using td.features.state;

namespace td.features.enemy.systems
{
    public class EnemyDiedService: IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<IEventBus> events;

        public void Init(IEcsSystems systems)
        {
            events.Value.Entity.ListenTo<Event_Enemy_ChangeHealth>(OnEnemyHelthChanged);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Entity.RemoveListener<Event_Enemy_ChangeHealth>(OnEnemyHelthChanged);
        }
        
        // ------------------------------------------------------------------ //

        private void OnEnemyHelthChanged(EcsPackedEntityWithWorld packedEntity, ref Event_Enemy_ChangeHealth @event)
        {
            if (!enemyService.Value.IsAlive(packedEntity, out var enemyEntity)) return;

            ref var enemy = ref enemyService.Value.GetEnemy(enemyEntity);

            if (enemy.health > 0.0001f) return;
            
            //todo тут можно запустиить анимацию смерти, эфекты, добавление очков и т.п.
            
            common.Value.SafeDelete(enemyEntity);
            state.Value.Energy += enemy.energy;
            state.Value.EnemiesCount--;
        }
    }
}