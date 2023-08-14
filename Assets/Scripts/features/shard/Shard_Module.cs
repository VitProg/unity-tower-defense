using System;
using Leopotam.EcsProto;
using td.features.shard.shardCollection;
using td.features.shard.shardStore;
using td.features.shard.systems;
using td.utils.ecs;
using UnityEngine;

namespace td.features.shard
{
    public class Shard_Module : IProtoModule
    {
        private readonly Func<float> getDeltaTime;
        
        public Shard_Module(Func<float> getDeltaTime)
        {
            // Debug.Log($"{GetType().Name} Init");
            this.getDeltaTime = getDeltaTime;
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new ShardUpdateAndInit_MB_System(1/20f, 0f, getDeltaTime))
                //
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
    }
}