using Leopotam.EcsProto;
using td.features.state;

namespace td.features.shard.shardStore
{
    public class ShardStore_Module : IProtoModuleWithStateEx
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new ShardStore_InitSystem())
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public IStateExtension StateEx() => new ShardStore_StateEx();
    }
}