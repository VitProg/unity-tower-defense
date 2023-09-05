using Leopotam.EcsProto;
using td.features.infoPanel.systems;
using td.features.state;
using td.features.state.interfaces;

namespace td.features.infoPanel
{
    public class InfoPanel_Module : IProtoModuleWithStateEx
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            
            systems
                .AddSystem(new InfoPanel_System())
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public IStateExtension StateEx() => new InfoPanel_State();
    }
}