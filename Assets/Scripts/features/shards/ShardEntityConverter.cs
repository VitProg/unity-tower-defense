using Leopotam.EcsLite;
using td.components.flags;
using td.components.refs;
using td.features.shards.mb;
using td.monoBehaviours;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.shards
{
    //ToDo
    public class ShardEntityConverter : IEntityConverter<Shard>
    {
        [InjectWorld] private EcsWorld world;
        
        public void Convert(GameObject gameObject, int entity)
        {
            ref var shard = ref world.GetComponent<Shard>(entity);
            
            world.GetComponent<OnlyOnLevel>(entity);
            world.GetComponent<Ref<GameObject>>(entity).reference = gameObject;
            
            var shardMonoBehavior = gameObject.GetComponent<ShardMonoBehaviour>();
            shard.red = shardMonoBehavior.red;
            shard.green = shardMonoBehavior.green;
            shard.blue = shardMonoBehavior.blue;
            shard.aquamarine = shardMonoBehavior.aquamarine;
            shard.yellow = shardMonoBehavior.yellow;
            shard.orange = shardMonoBehavior.orange;
            shard.pink = shardMonoBehavior.pink;
            shard.violet = shardMonoBehavior.violet;

            world.DelComponent<IsDisabled>(entity);
            world.DelComponent<IsDestroyed>(entity);
            
#if UNITY_EDITOR
            if (!gameObject.GetComponent<EcsComponentsInfo>())
            {
                gameObject.AddComponent<EcsComponentsInfo>();
            }
#endif
        }
    }
}