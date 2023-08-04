using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.fx.events;
using td.features.fx.types;
using td.features.goPool;
using td.features.spriteAnimator;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fx.effects.@base
{
    public class WithSpriteAnimatorFX_System<T> : IEcsInitSystem, IEcsDestroySystem where T : struct, IEntityFallowFX, IWithColorFX, IWithSpriteAnimatorFX
    {
        private readonly EcsInject<FX_Pools> pools;
        private readonly EcsInject<FX_Service> fxService;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Prefab_Service> prefabService;
        private readonly EcsInject<GameObjectPool_Service> goPoolService;
        private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
        
        private readonly EcsPoolInject<T> pool = Constants.Worlds.FX;

        public void Init(IEcsSystems systems)
        {
            events.Value.Entity.ListenTo<FX_Event_EnemyFallow_Spawned<T>>(OnSpawned);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Entity.RemoveListener<FX_Event_EnemyFallow_Spawned<T>>(OnSpawned);
        }
        
        // -----------------------------------------------------

        private void OnSpawned(EcsPackedEntityWithWorld packedentity, ref FX_Event_EnemyFallow_Spawned<T> @event)
        {
            var fxName = typeof(T).Name;
            
            Debug.Log($"{fxName} OnSpawned");
            
            if (!packedentity.Unpack(out var w, out var fxEntity) || w != fxWorld.Value) return;
            
            // todo

            ref var fx = ref pool.Value.Get(fxEntity);
            
            var prefab = prefabService.Value.GetPrefab(PrefabCategory.FX, string.IsNullOrEmpty(fx.PrefabName) ? "Exploding_Imploding_Lt" : fx.PrefabName);
            var goPool = goPoolService.Value.GetPool(
                prefab,
                fxService.Value.fxContainer.transform,
                10,
                100
            );

            ref var transform = ref pools.Value.withTransformPool.Value.Get(fxEntity);

            var go = goPool.Get().gameObject;
            fxService.Value.PrepareGO(go, fxEntity);
            go.transform.position = transform.position;
            go.transform.localScale = transform.scale;
            go.transform.rotation = transform.rotation;

            var sr = go.transform.GetComponent<SpriteRenderer>();
            if (!sr) sr = go.transform.GetComponentInChildren<SpriteRenderer>();
            if (sr)
            {
                Debug.Log($"{fxName} set color: {fx.Color}");
                
                sr.color = fx.Color;
            }
            else Debug.Log($"{fxName} SpriteRenderer not found ((");
            
            var anim = go.transform.GetComponent<SpriteAnimatorMB>();
            if (!anim) anim = go.transform.GetComponentInChildren<SpriteAnimatorMB>();
            if (anim)
            {
                if (fx.IsLoop.HasValue) anim.loop = fx.IsLoop.Value;
                if (fx.Speed is > 0f) anim.speed = fx.Speed.Value;
                if (fx.IsReverse.HasValue) anim.reverse = fx.IsReverse.Value;
            } else Debug.Log($"{fxName} SpriteAnimatorMB not found ((");

            pools.Value.refGOPoolFX.Value.SafeAdd(fxEntity).reference = go;
        }
    }
}