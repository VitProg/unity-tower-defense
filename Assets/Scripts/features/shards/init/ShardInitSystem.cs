using System;
using Leopotam.EcsLite;
using td.features.shards.mb;
using td.services.ecsConverter;
using td.utils.ecs;
using Object = UnityEngine.Object;

namespace td.features.shards.init
{
    public class ShardInitSystem : IEcsInitSystem
    {
        [Inject] private EntityConverters converters;
        
        public void Init(IEcsSystems systems)
        {
            foreach (var shard in Object.FindObjectsOfType<ShardMonoBehaviour>())
            {
                if (!converters.Convert<Shard>(shard.gameObject, out var entity))
                {
                    throw new NullReferenceException($"Failed to convert GameObject {shard.gameObject.name}");
                }
            }
        }
    }
}