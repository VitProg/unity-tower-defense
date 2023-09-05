using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.shard.bus;
using td.features.shard.data;
using td.features.shard.shardCollection;
using td.features.shard.shardStore;
using td.features.shard.systems;
using td.utils.ecs;
using UnityEngine;

namespace td.features.shard
{
    public class Shard_Module : IProtoModuleWithEvents
    {
        private readonly Func<float> getDeltaTime;
        private readonly Shards_Config_SO shardConfigSO;

        public Shard_Module(Func<float> getDeltaTime)
        {
            this.getDeltaTime = getDeltaTime;
            shardConfigSO = Resources.Load<Shards_Config_SO>("Configs/Shards Config");
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new Shard_UpdateAndInit_MB_System(1/30f, 0f, getDeltaTime))
                .AddSystem(new Shard_BuyHandler_System())
                .AddSystem(new Shard_CombineHandler_System())
                .AddSystem(new Shard_DropHandler_System())
                .AddSystem(new Shard_InsertHandler_System())
                //
                .AddService(shardConfigSO, true)
                .AddService(new Shard_Service(), true)
                .AddService(new Shard_Calculator(), true)
                .AddService(new Shard_Converter(), true)
                .AddService(new Shard_MB_Service(), true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new Shard_Aspect(),
            };
        }

        public IProtoModule[] Modules()
        {
            return new IProtoModule[]
            {
                new ShardCollection_Module(),
                new ShardStore_Module(),
            };
        }

        public Type[] Events() => Ev.E<
           Command_BuyShard,
           Command_CombineShards_InBuilding,
           Command_CombineShards_InCollection,
           Command_DropShard_OnMap,
           Command_InsertShard_InBuilding,
           //
           Event_ShardsCombined,
           Event_ShardDropped_OnMap,
           Event_ShardInserted_InBuilding
        >();
    }
}