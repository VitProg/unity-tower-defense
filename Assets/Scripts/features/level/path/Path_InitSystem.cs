using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level.bus;

namespace td.features.level
{
    public class Path_InitSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private Path_Service pathService;
        
        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_LevelPreLoaded>(OnLevelPreLoaded);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Event_LevelPreLoaded>(OnLevelPreLoaded);
        }
        
        // ----------------------------------------------------------------

        private void OnLevelPreLoaded(ref Event_LevelPreLoaded obj)
        {
            pathService.InitPath();
        }
    }
}