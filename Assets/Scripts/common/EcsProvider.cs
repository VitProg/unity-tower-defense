using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UniLeo.Providers;
using td.components;
using td.components.refs;
using td.monoBehaviours;
using UnityEngine;


namespace td.common
{
    [RequireComponent(typeof(EcsComponentsInfo))]
    public abstract class EcsProvider<TComponent> : BaseEcsProvider where TComponent : struct
    {
        public override void Convert(int e, EcsWorld w)
        {
            var pool = w.GetPool<TComponent>();

            if (pool.Has(e)) pool.Del(e);
            pool.Add(e) = component;

            if (GetType() != typeof(RefGameObjectProvider))
            {
                var r = GetComponent<RefGameObjectProvider>();
                if (r != null)
                {
                    var go = GetComponent<GameObject>();
                    r.component.reference = go;
                }
                Destroy(this);
            }
        }

        public TComponent component;
    }
}