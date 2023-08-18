using Leopotam.EcsProto.QoL;
using td.features.goPool;
using td.monoBehaviours;
using UnityEngine;

namespace td.utils
{
    public static class EcsPoolUtils
    {
        public static void ActionOnDestroy(PoolableObject o)
        {
            var ecsEntity = o.GetComponent<EcsEntity>();
            if (ecsEntity != null &&
                ecsEntity.packedEntity.HasValue &&
                ecsEntity.packedEntity.Value.Unpack(out var world , out var entity)
               )
            {
                world.DelEntity(entity);
            }

            Object.Destroy(o.gameObject);
        }
    }
}