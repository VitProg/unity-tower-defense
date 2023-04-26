using Leopotam.EcsLite;
using UnityEngine;

namespace td.services.ecsConverter
{
    public interface IConvertibleToEntity

    {
        public void UpdateFromEntity();
        public void UpdateEntity(EcsWorld world, int entity);
    }
}