using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common.bus;
using td.features.goPool;

namespace td.features._common.systems
{
    public class RemoveSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<GameObjectPool_Service> poolServise;
        private readonly EcsInject<Common_Pools> commonPools;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsWorldInject world;
        private readonly EcsInject<IEventBus> events;

        public void Init(IEcsSystems systems)
        {
            events.Value.Entity.ListenTo<Command_Remove>(OnRemoveCommand);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Entity.RemoveListener<Command_Remove>(OnRemoveCommand);
        }

        // --------------------------------------------------------- //
        
        private void OnRemoveCommand(EcsPackedEntityWithWorld packedEntity, ref Command_Remove _)
        {
            common.Value.Remove(packedEntity);
        }
    }
}