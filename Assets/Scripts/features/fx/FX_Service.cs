using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.UnityEditor;
using td.features.fx.subServices;
using td.monoBehaviours;
using td.utils.di;
using UnityEngine;

namespace td.features.fx
{
    public class FX_Service
    {
        [field: AutoResolve] public FX_EntityModifier_SubService EntityModifier { get; } = new();

        [field: AutoResolve]
        public FX_EntityFallow_SubService EntityFallow { get; } = new();
        
        [field: AutoResolve]
        public FX_Position_SubService Position { get; } = new();
        
        [field: AutoResolve]
        public FX_Screen_SubService Screen { get; } = new();

        public readonly GameObject fxContainer;

        public FX_Service()
        {
            fxContainer = new GameObject("[FX Container]");
        }

        internal void PrepareGO(GameObject go, int fxEntity)
        {
            var ecsEntity = go.GetComponent<EcsEntity>() ?? go.AddComponent<EcsEntity>();
            ecsEntity.packedEntity = fxWorld.Value.PackEntityWithWorld(fxEntity);
#if UNITY_EDITOR && DEBUG
            // if (!gameObject.GetComponent<EcsComponentsInfo>()) gameObject.AddComponent<EcsComponentsInfo>();
            if (!go.GetComponent<EcsEntityDebugView>())
            {
                var entityObserver = go.AddComponent<EcsEntityDebugView>();
                entityObserver.Entity = fxEntity;
                entityObserver.World = fxWorld.Value;
            }
#endif

        }

        public readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
    }
}