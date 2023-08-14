using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.window;

namespace td.features._common.systems
{
    public class SturtupInitSystem : IProtoPreInitSystem
    {
        [DI] protected Window_Service windowService; 
        
        public void PreInit(IProtoSystems systems)
        {
            var show = windowService.Open(Window_Service.Type.MainMenu, true);
        }
    }
}