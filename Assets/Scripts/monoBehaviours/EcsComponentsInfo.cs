using System.Collections.Generic;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UniLeo;
using UnityEngine;

namespace td.monoBehaviours
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ConvertToEntity))]
    public class EcsComponentsInfo : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector] public ConvertToEntity ConvertToEntity { get; private set; }
        [HideInInspector] public EcsWorld World { get; private set; }
        [HideInInspector] public EcsPackedEntity PackedEntity { get; private set; }
        [SerializeField] public bool Initialized { get; private set; }

        [HideInInspector] public string Data { get; private set; }
        [HideInInspector] public readonly Dictionary<string, object> Componnents = new Dictionary<string, object>();

        private void Start()
        {
            ConvertToEntity = GetComponent<ConvertToEntity>();

        }

        private void Update()
        {
            if (!Initialized)
            {
                Initialize();
                return;
            }
            
            Componnents.Clear();

            if (PackedEntity.Unpack(World, out var entity))
            {
                var components = new object[] { };
                World.GetComponents(entity, ref components);

                foreach (var component in components)
                {
                    if (component != null)
                    {
                        var nname = component.GetType().Name;
                        Componnents.Remove(name);
                        Componnents.Add(name, component);
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
            if (!ConvertToEntity.IsConverted)
            {
                return;
            }

            Initialized = true;

            World = ConvertToEntity.World;
            PackedEntity = ConvertToEntity.PackedEntity;
        }
#endif
    }
}
