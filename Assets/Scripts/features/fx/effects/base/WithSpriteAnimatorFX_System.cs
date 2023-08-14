using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.eventBus;
using td.features.fx.events;
using td.features.fx.types;
using td.features.goPool;
using td.features.prefab;
using td.features.spriteAnimator;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fx.effects.@base
{
    public class WithSpriteAnimatorFX_System<T> : IProtoInitSystem, IProtoDestroySystem where T : struct, IEntityFallowFX, IWithColorFX, IWithSpriteAnimatorFX
    {
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;
        [DI] private FX_Service fxService;
        [DI] private EventBus events;
        [DI] private Prefab_Service prefabService;
        [DI] private GOPool_Service goPoolService;
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<FX_Event_EnemyFallow_Spawned<T>>(OnSpawned);
        }

        public void Destroy()
        {
            events.global.RemoveListener<FX_Event_EnemyFallow_Spawned<T>>(OnSpawned);
        }
        
        // -----------------------------------------------------

        private void OnSpawned(ref FX_Event_EnemyFallow_Spawned<T> data)
        {
            var fxName = typeof(T).Name;
            
            if (!data.Entity.Unpack(out var w, out var fxEntity) || w != aspect.World()) return;

            var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
            
            ref var fx = ref pool.Get(fxEntity);
            
            var prefab = prefabService.GetPrefab(PrefabCategory.FX, string.IsNullOrEmpty(fx.PrefabName) ? "Exploding_Imploding_Lt" : fx.PrefabName);
            var goPool = goPoolService.GetPool(
                prefab,
                fxService.fxContainer.transform,
                10,
                100
            );

            ref var transform = ref aspect.withTransformPool.Get(fxEntity);

            var go = goPool.Get().gameObject;
            fxService.PrepareGO(go, fxEntity);
            go.transform.position = transform.position;
            go.transform.localScale = transform.scale;
            go.transform.rotation = transform.rotation;

            var sr = go.transform.GetComponent<SpriteRenderer>();
            if (!sr) sr = go.transform.GetComponentInChildren<SpriteRenderer>();
            if (sr)
            {
                sr.color = fx.Color;
            }
            
            var anim = go.transform.GetComponent<SpriteAnimatorMB>();
            if (!anim) anim = go.transform.GetComponentInChildren<SpriteAnimatorMB>();
            if (anim)
            {
                if (fx.IsLoop.HasValue) anim.loop = fx.IsLoop.Value;
                if (fx.Speed is > 0f) anim.speed = fx.Speed.Value;
                if (fx.IsReverse.HasValue) anim.reverse = fx.IsReverse.Value;
            } else Debug.Log($"{fxName} SpriteAnimatorMB not found ((");

            aspect.refGOPool.GetOrAdd(fxEntity).reference = go;
        }
    }
}