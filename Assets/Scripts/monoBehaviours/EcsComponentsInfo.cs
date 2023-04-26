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
        
        public EcsEntity ecsEntity { get; private set; }
        private string data;
        private readonly Dictionary<string, object> componnents = new();

        private void Start()
        {
            ecsEntity = GetComponent<EcsEntity>();
        }

        private void Update()
        {
            if (world == null)
            {
                DI.Resolve(this);
                Initialize();
            }
            
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

        private void Initialize()
        {
            
            // if (!ConvertToEntity.IsConverted)
            // {
            //     return;
            // }
            //
            // Initialized = true;
            //
            // World = ConvertToEntity.World;
            // PackedEntity = ConvertToEntity.PackedEntity;
        }
#endif
    }
}
