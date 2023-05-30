using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.features.shards.commands;
using td.utils.ecs;
using Vector3 = UnityEngine.Vector3;

namespace td.features.shards.executors
{
    public class UIShardStoreShowHideExecutor : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;
        [InjectShared] private SharedData shared;
        
        private readonly EcsFilterInject<Inc<UIShowShardStoreOuterCommand>> showEntities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<UIHideShardStoreOuterCommand>> hideEntities = Constants.Worlds.Outer;
        
        public void Run(IEcsSystems systems)
        {
            if (shared.shardStore != null)
            {
                foreach (var showEntity in showEntities.Value)
                {
                    Show(showEntities.Pools.Inc1.Get(showEntity).x);
                    break;
                }

                if (hideEntities.Value.GetEntitiesCount() > 0)
                {
                    Hide();
                }
            }

            systems.CleanupOuter(showEntities);
            systems.CleanupOuter(hideEntities);
        }

        private void Show(float x)
        {
            var ui = shared.shardStore;
            var transform = shared.shardStore.transform;
            var position = transform.position;
            transform.position = new Vector3(x, position.y, position.z);
            ui.gameObject.SetActive(true);
        }

        private void Hide()
        {
            var ui = shared.shardStore;
            ui.gameObject.SetActive(false);
        }
    }
}