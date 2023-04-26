using Leopotam.EcsLite;

namespace td.common
{
    public interface ICopyableComponent
    {
        void CopyToEntity(EcsWorld world, int entity);
    }
}