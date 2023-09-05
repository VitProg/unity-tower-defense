using Leopotam.EcsProto;
using td.features.shard.shardCollection.systems;
using td.features.shard.systems;
using td.features.state;
using td.features.state.interfaces;

namespace td.features.shard.shardCollection
{
    public class ShardCollection_Module : IProtoModuleWithStateEx
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new ShardCollection_Initialize_System())
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

        public IStateExtension StateEx() => new ShardCollection_State();
    }
}