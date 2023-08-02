using Leopotam.EcsLite;

namespace td.features._common
{
    public class EcsWorldsStorage_Service
    {
        public readonly EcsWorld world;
        public readonly EcsWorld eventsWorld;

        public EcsWorldsStorage_Service(EcsWorld _world, EcsWorld _eventsWorld)
        {
            world = _world;
            eventsWorld = _eventsWorld;
        }
    }
}