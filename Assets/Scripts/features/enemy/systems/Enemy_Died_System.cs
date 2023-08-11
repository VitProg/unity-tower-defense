using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.enemy.bus;
using td.features.eventBus;
using td.features.state;

namespace td.features.enemy.systems
{
    public class Enemy_Died_Service : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private Enemy_Service enemyService;
        [DI] private Common_Service common;
        [DI] private State state;
        [DI] private EventBus events;

        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Event_Enemy_ChangeHealth>(OnEnemyHelthChanged);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Event_Enemy_ChangeHealth>(OnEnemyHelthChanged);
        }
        
        // ------------------------------------------------------------------ //

        private void OnEnemyHelthChanged(ref Event_Enemy_ChangeHealth ev)
        {
            if (!enemyService.IsAlive(ev.Entity, out var enemyEntity)) return;

            ref var enemy = ref enemyService.GetEnemy(enemyEntity);

            if (enemy.health > 0.0001f) return;
            
            //todo тут можно запустиить анимацию смерти, эфекты, добавление очков и т.п.
            
            common.SafeDelete(enemyEntity);
            state.Energy += enemy.energy;
            state.EnemiesCount--;
        }
    }
}