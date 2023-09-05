using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy;
using td.features.enemy.bus;
using td.features.eventBus;
using td.features.state;

namespace td.features.enemy.systems
{
    public class Enemy_Died_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private Enemy_Aspect aspect;
        [DI] private Enemy_Service enemyService;
        [DI] private Destroy_Service destroyService;
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
            
            destroyService.MarkAsRemoved(aspect.World().PackEntityWithWorld(enemyEntity));

            state.IncreaseEnergy(enemy.energy);
            //state.ReduceEnemiesCount();
            state.IncreaseKillsCount();
            
            events.global.Add<Event_Enemy_Died>().Entity = ev.Entity;
        }
    }
}