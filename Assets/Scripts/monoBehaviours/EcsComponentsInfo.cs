using System.Collections.Generic;
using Leopotam.EcsLite;
using td.utils.ecs;
// using Mitfart.LeoECSLite.UniLeo;
using UnityEngine;

namespace td.monoBehaviours
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    public class EcsComponentsInfo : MonoBehaviour
    {
#if UNITY_EDITOR
        [InjectWorld] private EcsWorld world;

        private EcsEntity ecsEntityInternal;

        public EcsEntity ecsEntity
        {
            get
            {
                if (ecsEntityInternal == null)
                {
                    ecsEntityInternal = GetComponent<EcsEntity>();
                }

                return ecsEntityInternal;
            }
        }

        private string data;
        private readonly Dictionary<string, object> componnents = new();

        private bool diResolved;
        
        public async void Awake()
        {
            await DI.Resolve(this);
            diResolved = true;
        }

        private void Start()
        {
            ecsEntityInternal = GetComponent<EcsEntity>();
        }

        private void Update()
        {
            if (!diResolved) return;
            
            componnents.Clear();

            if (ecsEntity.TryGetEntity(out var entity))
            {
                var components = new object[] { };
                world.GetComponents(entity, ref components);

                foreach (var component in components)
                {
                    if (component != null)
                    {
                        var nname = component.GetType().Name;
                        componnents.Remove(name);
                        componnents.Add(name, component);
                        // Data += "-----\n" +
                        // component.GetType().Name +
                        // ":\n" +
                        // JsonUtility.ToJson(component, true);
                    }
                }
            }
        }
#endif
    }
}
