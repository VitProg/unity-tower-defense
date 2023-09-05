using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy;
using td.features.enemy.bus;
using td.features.eventBus;

namespace td.features.fx.listeners
{
    public class FX_Clear_ListenerSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private Enemy_Service enemyService;
        [DI] private FX_Service fxService;
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Event_Enemy_Died>(OnEnemyDied);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Event_Enemy_Died>(OnEnemyDied);
        }
        
        // ----------------------------------------------------------------

        private void OnEnemyDied(ref Event_Enemy_Died ev)
        {
            if (!ev.Entity.Unpack(out var world, out var enemyEntity) || !enemyService.IsDead(enemyEntity)) return;
            
            fxService.entityFallow.RemoveAll(ev.Entity);            
            fxService.entityModifier.RemoveAll(ev.Entity);            
        }
    }
}