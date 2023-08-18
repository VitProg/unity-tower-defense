using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.goPool;
using td.features.state;
using td.monoBehaviours;
using UnityEngine;

namespace td.features.fx.systems
{
    public class FX_AutoRemoveSystem : IProtoRunSystem
    {
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;
        [DI] private GOPool_Service goPoolService;
        [DI] private State state;

        public void Run()
        {
            // if (!state.GetGameSimulationEnabled()) return;
            
            foreach (var fxEntity in aspect.itNeedRemove)
            {
                ref var rem = ref aspect.needRemovePool.Get(fxEntity);

                if (!rem.now)
                {
                    rem.now = true;
                    continue;
                }
                
                // remove fx
                if (aspect.refGOPool.Has(fxEntity))
                {
                    var go = aspect.refGOPool.Get(fxEntity).reference;
                    if (go != null)
                    {
                        var poolableObject = go.GetComponent<PoolableObject>();
                        if (poolableObject != null)
                        {
                            goPoolService.Release(poolableObject);
                        }
                        else
                        {
                            Object.Destroy(go);
                        }
                    }
                }
                aspect.World().DelEntity(fxEntity);
            }
        }
    }
}