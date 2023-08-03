using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.fx.flags;
using UnityEngine;

namespace td.features.fx.systems
{
    public class FX_AutoRemoveSystem : IEcsRunSystem
    {
        private readonly EcsInject<FX_Pools> pools;
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
                    // ToDo: add rotation GO with GO Pool
                    Object.Destroy(pools.Value.refGOPoolFX.Value.Get(fxEntity).reference);
                }
                
                fxWorld.Value.DelEntity(fxEntity);
            }
        }
    }
}