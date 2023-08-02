using Leopotam.EcsLite;

namespace td.features.ecsConverter
{
    public interface IConvertibleToEntity

    {
        public void UpdateFromEntity();
        public void UpdateEntity(EcsWorld world, int entity);
    }
}