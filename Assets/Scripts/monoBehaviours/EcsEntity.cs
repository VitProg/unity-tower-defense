using Leopotam.EcsLite;
using NaughtyAttributes;
using td.utils.ecs;
using UnityEngine;

namespace td.monoBehaviours
{
    public class EcsEntity : MonoBehaviour
    {
        [SerializeField] public EcsPackedEntity? PackedEntity = null;

        [ShowNativeProperty] public bool HasEntity => PackedEntity.HasValue;

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