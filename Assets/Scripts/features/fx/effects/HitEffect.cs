using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using NaughtyAttributes;
using td.features._common;
using td.features.fx.events;
using td.features.fx.types;
using td.features.goPool;
using td.features.spriteAnimator;
using td.monoBehaviours;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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

    public class HitFX_System : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<FX_Pools> pools;
        private readonly EcsInject<FX_Service> fxService;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Prefab_Service> prefabService;
        private readonly EcsInject<GameObjectPool_Service> goPoolService;
        
        private readonly EcsPoolInject<HitFX> pool = Constants.Worlds.FX;
        
        private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
        
        private GameObject prefab;
        private ObjectPool<PoolableObject> goPool;
        
        public void Init(IEcsSystems systems)
        {
            events.Value.Entity.ListenTo<FX_Event_EnemyFallow_Spawned<HitFX>>(OnSpawned);
            prefab = prefabService.Value.GetPrefab(PrefabCategory.FX, "HitFX");
            goPool = goPoolService.Value.GetPool(
                prefab,
                fxService.Value.fxContainer.transform,
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
        

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Entity.RemoveListener<FX_Event_EnemyFallow_Spawned<HitFX>>(OnSpawned);
        }

        // -----------------------------------------------------

        private void OnSpawned(EcsPackedEntityWithWorld fxPackedEntity, ref FX_Event_EnemyFallow_Spawned<HitFX> @event)
        {
            Debug.Log("HitFX OnSpawned");
            
            if (!fxPackedEntity.Unpack(out var w, out var fxEntity) || w != fxWorld.Value) return;
            
            // todo

            ref var fx = ref pool.Value.Get(fxEntity);
            ref var transform = ref pools.Value.withTransformPool.Value.Get(fxEntity);
            
            var go = goPool.Get().gameObject;
            fxService.Value.PrepareGO(go, fxEntity);
            
            var fxmb = go.transform.GetComponent<HitEffect>();
            if (!fxmb || !fxmb.animator || !fxmb.spriteRenderer) throw new Exception("FX dasn't valid!");

            fxmb.Color = fx.Color;
            fxmb.animator.OnFinish.AddListener(delegate { fxService.Value.EntityFallow.Remove<HitFX>(fxEntity); });

            transform.rotation = RandomUtils.Rotation();
            transform.scale *= RandomUtils.Range(0.5f, 1.0f);
            go.transform.rotation = transform.rotation;
            go.transform.localScale = transform.scale;
            
            fxmb.animator.speed *= RandomUtils.Range(0.85f, 1.5f);
            fxmb.animator.Play();
            
            pools.Value.refGOPoolFX.Value.SafeAdd(fxEntity).reference = go;
        }
    }
}