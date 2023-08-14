using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.destroy;
using td.features.ecsConverter;
using td.features.shard.mb;
using UnityEngine;

namespace td.features.shard
{
    public class Shard_Converter : BaseEntity_Converter
    {
        [DI] private Shard_Aspect aspect;
        [DI] private Shard_Service shardService;
        [DI] private Shard_MB_Service shardMBService;
        [DI] private Destroy_Service destroyService;

        public override ProtoWorld World() => aspect.World();

        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);

            ref var shard = ref shardService.GetShard(entity);
            shard._id_ = shard._id_ > 0 ? shard._id_ : CommonUtils.ID("shard-converter");
            
            destroyService.SetIsOnlyOnLevel(entity, true);
            
            var shardMB = 
                gameObject.GetComponent<ShardMonoBehaviour>() ??
                gameObject.GetComponentInChildren<ShardMonoBehaviour>();
            
            shardService.GetShardMBRef(entity).reference = shardMB;
            shardMBService.Add(shardMB);
        }
    }
}