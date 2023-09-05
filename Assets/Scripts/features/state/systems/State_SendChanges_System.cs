using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace td.features.state.systems
{
    public class State_SendChanges_System : IProtoPostRunSystem
    {
        [DI] private State state;
        
        public void PostRun()
        {
            state.SendChanges();
        }
    }
}