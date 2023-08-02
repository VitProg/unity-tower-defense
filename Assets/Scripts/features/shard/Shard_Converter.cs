using Leopotam.EcsLite;
using td.features._common;
using td.features.ecsConverter;
using td.features.shard.components;
using td.features.shard.mb;
using UnityEngine;

namespace td.features.shard
{
    public class Shard_Converter : BaseEntity_Converter
    {
        private readonly EcsInject<Shard_Service> shardService;
        private readonly EcsInject<MB_Shard_Service> mbShardService;
        private readonly EcsInject<Common_Service> common;

        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);

            ref var shard = ref shardService.Value.GetShard(entity);
            shard._id_ = shard._id_ > 0 ? shard._id_ : CommonUtils.ID("shard-converter");
            
            common.Value.SetIsOnlyOnLevel(entity, true);
            
            var shardMB = gameObject.GetComponent<ShardMonoBehaviour>() ?? gameObject.GetComponentInChildren<ShardMonoBehaviour>();
            shardService.Value.GetShardMBRef(entity).reference = shardMB;
            mbShardService.Value.Add(shardMB);
        }
    }
}