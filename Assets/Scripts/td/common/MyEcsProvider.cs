using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UniLeo.Providers;


namespace td.common
{
    public abstract class MyEcsProvider<TComponent> : BaseEcsProvider where TComponent : struct{
        public TComponent component;

        private bool converted;
        
        private EcsPackedEntity packedEntity;
        private EcsWorld world;
        private EcsPool<TComponent> pool;

        public override void Convert(int e, EcsWorld w){
            world = w;

            pool = world.GetPool<TComponent>();
         
            if (pool.Has(e)) pool.Del(e);
            pool.Add(e) = component;

            packedEntity = world.PackEntity(e);

            converted = true;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!converted || !packedEntity.Unpack(world, out var entity)) return;
            
            if (pool.Has(entity))
            {
                ref var cmp = ref pool.Get(entity);

                component = cmp;
        
                // var cmpType = cmp.GetType();
                // // Debug.Log(cmpType.GetFields());
                //
                // foreach (var field in cmpType.GetFields())
                // {
                //     var value = field.GetValue(cmp);
                //     field.SetValue(component, value);
                // }
        
            }
        }
    }
#endif
}