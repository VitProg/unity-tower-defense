using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.building.components;
using td.features.destroy.flags;

namespace td.features.building
{
    public class Building_Aspect : ProtoAspectInject
    {
        public ProtoPool<Building> buildingPool;

        public ProtoItExc it = new(
            It.Inc<Building>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
    }
}