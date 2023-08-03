using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.fx.events;
using td.features.fx.types;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.fx.effects
{
    [Serializable]
    public struct PoisonStatusFX : IEntityFallowFX
    {
        public Color color;
    }

    public class PoisonStatusFX_System : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<FX_Pools> pools;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Prefab_Service> prefabService;
        private readonly EcsPoolInject<PoisonStatusFX> pool = Constants.Worlds.FX;
        
        private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
        private GameObject prefab;

        public void Init(IEcsSystems systems)
        {
            events.Value.Entity.ListenTo<FX_Event_EnemyFallow_Spawned<PoisonStatusFX>>(OnSpawned);
            
            prefab = prefabService.Value.GetPrefab(PrefabCategory.FX, "PoisonStatusFX MyAnim");
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Entity.RemoveListener<FX_Event_EnemyFallow_Spawned<PoisonStatusFX>>(OnSpawned);
        }
        
        // -----------------------------------------------------

        private void OnSpawned(EcsPackedEntityWithWorld packedentity, ref FX_Event_EnemyFallow_Spawned<PoisonStatusFX> @event)
        {
            Debug.Log("PoisonStatusFX OnSpawned");
            
            if (!packedentity.Unpack(out var w, out var fxEntity) || w != fxWorld.Value) return;
            
            // todo
            
            ref var fx = ref pool.Value.Get(fxEntity);

            ref var transform = ref pools.Value.withTransformPool.Value.Get(fxEntity);

            var go = Object.Instantiate(prefab, transform.position, Quaternion.identity);
            var sr = go.transform.GetComponent<SpriteRenderer>();
            if (sr)
            {
                sr.color = fx.color;
            }
            
            pools.Value.refGOPoolFX.Value.SafeAdd(fxEntity).reference = go;
        }
    }
}