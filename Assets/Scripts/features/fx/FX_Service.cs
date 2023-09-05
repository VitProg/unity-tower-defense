using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.features.fx.subServices;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fx
{
    public class FX_Service
    {
        [DI] public FX_EntityModifier_SubService entityModifier;
        [DI] public FX_EntityFallow_SubService entityFallow;
        [DI] public FX_Position_SubService position;
        [DI] public FX_Screen_SubService screen;
        
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;

        public readonly GameObject fxContainer;

        public FX_Service()
        {
            fxContainer = GameObject.FindGameObjectWithTag(Constants.Tags.FXContainer);
        }

        internal void PrepareGO(GameObject go, int fxEntity)
        {
            var ecsEntity = go.GetComponent<EcsEntity>() ?? go.AddComponent<EcsEntity>();
            ecsEntity.packedEntity = aspect.World().PackEntityWithWorld(fxEntity);
#if UNITY_EDITOR && DEBUG
            // if (!gameObject.GetComponent<EcsComponentsInfo>()) gameObject.AddComponent<EcsComponentsInfo>();
            if (!go.GetComponent<ProtoEntityDebugView>())
            {
                var entityObserver = go.AddComponent<ProtoEntityDebugView>();
                entityObserver.Entity = fxEntity;
                entityObserver.World = aspect.World();
            }
#endif
        }
    }
}