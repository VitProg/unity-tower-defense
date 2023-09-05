using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level.bus;

namespace td.features.enemy.enemyPath
{
    public class EnemyPath_CacheRoutes_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private EnemyPath_Service enemyPathService;
        
        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_LevelLoaded>(OnLevelLoaded);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Event_LevelLoaded>(OnLevelLoaded);
        }

        private void OnLevelLoaded(ref Event_LevelLoaded obj)
        {
            enemyPathService.PrecalculateAllPaths();
        }
    }
}