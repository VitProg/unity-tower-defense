using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy.bus;
using td.features.eventBus;

namespace td.features.destroy.systems
{
    public class RemoveSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private Destroy_Service destroyService;
        [DI] private EventBus events;

        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Command_Remove>(OnRemoveCommand);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Command_Remove>(OnRemoveCommand);
        }

        // --------------------------------------------------------- //
        
        private void OnRemoveCommand(ref Command_Remove ev)
        {
            destroyService.SafeRemove(ev.Entity);
        }
    }
}