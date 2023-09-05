using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy.bus;
using td.features.eventBus;

namespace td.features.impactEnemy.systems
{
    public class ImpactEnemy_ClearSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private ImpactEnemy_Service impactEnemyService;
        
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
            if (ev.Entity.Unpack(out var world, out var entity))
            {
                impactEnemyService.RemoveAllDebuffs(entity);
            }
        }
    }
}