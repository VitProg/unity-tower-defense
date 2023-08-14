using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.impactKernel.bus;
using td.features.state;
using td.utils;

namespace td.features.impactKernel
{
    public class KernalChangeLivesSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private EventBus events;
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Command_Kernel_Damage>(OnKernemDamage);
            events.global.ListenTo<Command_Kernel_Heal>(OnKernelHeal);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Command_Kernel_Damage>(OnKernemDamage);
            events.global.RemoveListener<Command_Kernel_Heal>(OnKernelHeal);
        }
        
        //------------------------------------------//

        private void OnKernemDamage(ref Command_Kernel_Damage command)
        {
            state.SetLives(state.GetLives() - command.damage);
            if (FloatUtils.IsZero(state.GetLives()))
            {
                // ToDo: events.Value.Unique.Add<LevelFiled>();
            }
        }

        private void OnKernelHeal(ref Command_Kernel_Heal command)
        {
            state.SetLives(state.GetLives() + command.heal);
        }
    }
}