using Leopotam.EcsLite;
using td.common.ecs;

namespace td.components.events
{
    public struct ReachingTargetEvent : IEcsDoNotDebugLog<ReachingTargetEvent>
    {
        public EcsPackedEntity TargetEntity;
    }
}