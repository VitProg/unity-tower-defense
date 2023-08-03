using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.fx.events;
using td.features.fx.types;
using td.features.spriteAnimator;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace td.features.fx.effects
{
    [Serializable]
    public struct PoisonFX : IEntityFallowFX
    {
        public Color color;
    }

    public class PoisonFX_System : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<FX_Pools> pools;
        private readonly EcsInject<FX_Service> fxService;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Prefab_Service> prefabService;
        private readonly EcsPoolInject<PoisonFX> pool = Constants.Worlds.FX;
        
        private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
        private GameObject prefab;

        public void Init(IEcsSystems systems)
        {
            // events.Value.Entity.ListenTo<FX_Event_Position_Spawned<PoisonFX>>(OnSpawned);
            events.Value.Entity.ListenTo<FX_Event_EnemyFallow_Spawned<PoisonFX>>(OnSpawned);
            
            prefab = prefabService.Value.GetPrefab(PrefabCategory.FX, "PoisonBlinkFX MyAnim");
        }
        

        public void Destroy(IEcsSystems systems)
        {
            // events.Value.Entity.RemoveListener<FX_Event_Position_Spawned<PoisonFX>>(OnSpawned);
            events.Value.Entity.RemoveListener<FX_Event_EnemyFallow_Spawned<PoisonFX>>(OnSpawned);
        }

        // -----------------------------------------------------

        private void OnSpawned(EcsPackedEntityWithWorld packedentity, ref FX_Event_EnemyFallow_Spawned<PoisonFX> @event)
        {
            Debug.Log("PoisonFX OnSpawned");
            
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
            var anim = go.transform.GetComponent<SpriteAnimatorMB>() ?? go.transform.GetComponentInChildren<SpriteAnimatorMB>();
            if (anim)
            {
                anim.OnFinish.AddListener(delegate
                {
                    fxService.Value.EntityFallow.Remove<PoisonFX>(fxEntity);
                });
            }

            transform.rotation = Random.rotation;
            go.transform.rotation = transform.rotation;
            pools.Value.refGOPoolFX.Value.SafeAdd(fxEntity).reference = go;
        }
    }
}