using Leopotam.EcsLite;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;

namespace td.services
{
    public static class EcsPoolUtils
    {
        public static void ActionOnDestroy(PoolableObject o)
        {
            var world = DI.GetWorld();
            var ecsEntity = o.GetComponent<EcsEntity>();
            if (ecsEntity != null &&
                ecsEntity.PackedEntity.HasValue &&
                ecsEntity.PackedEntity.Value.Unpack(world, out var entity)
               )
            {
                world.DelEntity(entity);
            }

            Object.Destroy(o.gameObject);
        }
    }
}