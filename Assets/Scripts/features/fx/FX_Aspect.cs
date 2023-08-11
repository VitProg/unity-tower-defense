using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.fx.flags;
using td.features.fx.types;
using UnityEngine;

namespace td.features.fx
{
    public class FX_Pools
    {
        public readonly EcsPoolInject<IsPositionFX> isPositionPool = Constants.Worlds.FX;
        public readonly EcsPoolInject<IsScreenFX> isScreenPool = Constants.Worlds.FX;
        public readonly EcsPoolInject<IsEntityFallowFX> isEntityFallowPool = Constants.Worlds.FX;
        public readonly EcsPoolInject<IsEntityModifierFX> isEntityModifierPool = Constants.Worlds.FX;
        
        public readonly EcsPoolInject<WithDurationFX> withDurationPool = Constants.Worlds.FX;
        public readonly EcsPoolInject<WithTargetEntityFX> withTargetEntityPool = Constants.Worlds.FX;
        public readonly EcsPoolInject<WithTransformFX> withTransformPool = Constants.Worlds.FX;
        
        public readonly EcsPoolInject<NeedRemoveFX> needRemovePool = Constants.Worlds.FX; 
        
        public readonly EcsPoolInject<Ref<GameObject>> refGOPoolFX = Constants.Worlds.FX;

        internal readonly EcsFilterInject<Inc<IsEntityModifierFX, WithTargetEntityFX, WithDurationFX>, ExcludeNotAlive> entityModifierFilter = Constants.Worlds.FX;
        internal readonly EcsFilterInject<Inc<IsEntityFallowFX, WithTargetEntityFX, WithDurationFX, WithTransformFX>, ExcludeNotAlive> entityFallowFilter = Constants.Worlds.FX;
        internal readonly EcsFilterInject<Inc<IsPositionFX, WithTransformFX, WithDurationFX>, ExcludeNotAlive> positionFilter = Constants.Worlds.FX;
        internal readonly EcsFilterInject<Inc<IsScreenFX, WithTransformFX, WithDurationFX>, ExcludeNotAlive> screenFilter = Constants.Worlds.FX;
        
        internal readonly EcsFilterInject<Inc<WithTransformFX>, ExcludeNotAlive> withTransformFilter = Constants.Worlds.FX;
        internal readonly EcsFilterInject<Inc<WithTargetEntityFX>, ExcludeNotAlive> withTargetEntityFilter = Constants.Worlds.FX;
        internal readonly EcsFilterInject<Inc<WithDurationFX>, ExcludeNotAlive> withDurationFilter = Constants.Worlds.FX;
    }
}