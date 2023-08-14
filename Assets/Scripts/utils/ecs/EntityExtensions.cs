using Leopotam.EcsProto.QoL;

namespace td.utils.ecs
{
    public static class EntityExtensions
    {
        public static void DelEntity(this ProtoPackedEntityWithWorld packedEntity)
        {
            if (packedEntity.Unpack(out var world, out var entity)) return;
            world.DelEntity(entity);
        }
    }
}