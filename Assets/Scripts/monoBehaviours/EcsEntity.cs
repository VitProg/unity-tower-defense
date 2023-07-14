using Leopotam.EcsLite;
using NaughtyAttributes;
using td.utils.ecs;
using UnityEditor;
using UnityEngine;

namespace td.monoBehaviours
{
    public class EcsEntity : MonoBehaviour
    {
        [SerializeField] public EcsPackedEntity? PackedEntity = null;

        [ShowNativeProperty] public bool HasEntity => PackedEntity.HasValue;

#if UNITY_EDITOR
        [ReadOnly] public int entityID = -1;
        [ReadOnly] public string world = "";

        [Button("Unpack Entity")]
        public void Unpack()
        {
            if (!PackedEntity.HasValue)
            {
                world = "";
                entityID = -1;
                return;
            }

            world = "default";
            if (PackedEntity.Value.Unpack(DI.GetWorld(), out entityID)) return;
            world = "outer";
            if (PackedEntity.Value.Unpack(DI.GetWorld(Constants.Worlds.Outer), out entityID)) return;
            // world = "ui";
            // PackedEntity.Value.Unpack(DI.GetWorld(Constants.Worlds.UI), out entityID);
        } 
#endif
        
        public bool TryGetEntity(out int entity)
        {
            if (PackedEntity != null && PackedEntity.Value.Unpack(DI.GetWorld(), out entity))
            {
                return true;
            }

            entity = -1;
            return false;
        }
    }
}