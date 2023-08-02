using Leopotam.EcsLite;
using td.features.level.cells;
using td.features.shard;
using td.features.shard.components;
using td.features.state;
using td.monoBehaviours;

namespace td.features.tower
{
    public class MB_Tower_Service
    {
        private readonly EcsInject<Tower_Service> towerService;
        private readonly EcsInject<Shard_Service> shardService;
        private readonly EcsInject<ShardCalculator> calc;
        private readonly EcsInject<IState> state;


    }
}