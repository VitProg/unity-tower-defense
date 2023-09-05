using Leopotam.EcsProto.QoL;
using td.features.goPool;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile {
    public static class Projectile_GOPoolUtils {
        public static void ActionOnDestroy(PoolableObject o) {
            var ecsEntity = o.GetComponent<EcsEntity>();
            if (ecsEntity.packedEntity.Unpack(out var world, out var entity)) {
                world.DelEntity(entity);
            }

            Object.Destroy(o.gameObject);
        }
    }
}
