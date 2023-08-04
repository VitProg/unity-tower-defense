using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.fx.flags;
using td.features.goPool;
using td.monoBehaviours;
using UnityEngine;

namespace td.features.fx.systems
{
    public class FX_AutoRemoveSystem : IEcsRunSystem
    {
        private readonly EcsInject<FX_Pools> pools;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<GameObjectPool_Service> goPoolService;
        
        private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;

        private readonly EcsFilterInject<Inc<NeedRemoveFX>, ExcludeNotAlive> filter = Constants.Worlds.FX;

        public void Run(IEcsSystems systems)
        {
            foreach (var fxEntity in filter.Value)
            {
                ref var rem = ref filter.Pools.Inc1.Get(fxEntity);

                if (!rem.now)
                {
                    rem.now = true;
                    continue;
                }
                
                // remove fx
                if (pools.Value.refGOPoolFX.Value.Has(fxEntity))
                {
                    var go = pools.Value.refGOPoolFX.Value.Get(fxEntity).reference;
                    var poolableObject = go.GetComponent<PoolableObject>();
                    if (poolableObject != null)
                    {
                        goPoolService.Value.Release(poolableObject);
                    }
                    else
                    {
                        Object.Destroy(go);
                    }
                }
                fxWorld.Value.DelEntity(fxEntity);
            }
        }
    }
}