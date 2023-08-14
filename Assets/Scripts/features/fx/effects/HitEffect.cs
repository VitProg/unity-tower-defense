using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using NaughtyAttributes;
using td.features._common;
using td.features.eventBus;
using td.features.fx.events;
using td.features.fx.types;
using td.features.goPool;
using td.features.prefab;
using td.features.spriteAnimator;
using td.monoBehaviours;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Pool;

namespace td.features.fx.effects
{
    [Serializable]
    public struct HitFX : IEntityFallowFX, IWithColorFX
    {
        public Color Color { get; set; }
    }

    public class HitEffect : MonoBehaviour
    {
        [Required] public SpriteAnimatorMB animator;
        [Required] public SpriteRenderer spriteRenderer;
        [ShowNativeProperty] public Color Color
        {
            get => spriteRenderer.color;
            set
            {
                var c = value;
                c.a = spriteRenderer.color.a;
                spriteRenderer.color = c;
            }
        }

        [ShowNativeProperty]
        public float Alpha
        {
            get => spriteRenderer.color.a;
            set
            {
                var c = spriteRenderer.color;
                c.a = value;
                spriteRenderer.color = c;
            }
        }
    }

    public class HitFX_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;
        [DI] private FX_Service fxService;
        [DI] private EventBus events;
        [DI] private Prefab_Service prefabService;
        [DI] private GOPool_Service goPoolService;
        
        private GameObject prefab;
        private ObjectPool<PoolableObject> goPool;
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<FX_Event_EnemyFallow_Spawned<HitFX>>(OnSpawned);
            
            prefab = prefabService.GetPrefab(PrefabCategory.FX, "HitFX");
            goPool = goPoolService.GetPool(
                prefab,
                fxService.fxContainer.transform,
                10,
                100,
                null,
                null,
                delegate(PoolableObject go)
                {
                    go.gameObject.SetActive(false);
                    go.transform.GetComponent<HitEffect>().animator.Stop();
                    var fxmb = go.transform.GetComponent<HitEffect>();
                    fxmb.animator.OnFinish.RemoveAllListeners();
                }
            );
        }
        

        public void Destroy()
        {
            events.global.RemoveListener<FX_Event_EnemyFallow_Spawned<HitFX>>(OnSpawned);
        }

        // -----------------------------------------------------

        private void OnSpawned(ref FX_Event_EnemyFallow_Spawned<HitFX> data)
        {
            if (!data.Entity.Unpack(out var w, out var fxEntity) || w != aspect.World()) return;
            
            var pool = (ProtoPool<HitFX>)aspect.World().Pool(typeof(HitFX));
            
            ref var fx = ref pool.Get(fxEntity);
            ref var transform = ref aspect.withTransformPool.Get(fxEntity);
            
            var go = goPool.Get().gameObject;
            fxService.PrepareGO(go, fxEntity);
            
            var fxmb = go.transform.GetComponent<HitEffect>();
            if (!fxmb || !fxmb.animator || !fxmb.spriteRenderer) throw new Exception("FX dasn't valid!");

            fxmb.Color = fx.Color;
            fxmb.animator.OnFinish.AddListener(delegate
            {
                fxService.entityFallow.Remove<HitFX>(fxEntity);
                fxmb.animator.OnFinish.RemoveAllListeners();
            });

            transform.rotation = RandomUtils.Rotation();
            transform.scale *= RandomUtils.Range(0.5f, 1.0f);
            go.transform.rotation = transform.rotation;
            go.transform.localScale = transform.scale;
            
            fxmb.animator.speed *= RandomUtils.Range(0.85f, 1.5f);
            fxmb.animator.Play();
            
            aspect.refGOPool.GetOrAdd(fxEntity).reference = go;
        }
    }
}