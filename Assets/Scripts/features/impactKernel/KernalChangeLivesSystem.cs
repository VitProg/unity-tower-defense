using Leopotam.EcsLite;
using td.features.impactKernel.bus;
using td.features.state;
using td.utils;

namespace td.features.impactKernel
{
    public class KernalChangeLivesSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;
        
        public void Init(IEcsSystems systems)
        {
            events.Value.Global.SubscribeTo<Command_Kernel_Damage>(OnKernemDamage);
            events.Value.Global.SubscribeTo<Command_Kernel_Heal>(OnKernelHeal);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Global.RemoveListener<Command_Kernel_Damage>(OnKernemDamage);
            events.Value.Global.RemoveListener<Command_Kernel_Heal>(OnKernelHeal);
        }
        
        //------------------------------------------//

        private void OnKernemDamage(ref Command_Kernel_Damage command)
        {
            state.Value.Lives -= command.damage;
            if (FloatUtils.IsZero(state.Value.Lives))
            {
                // ToDo: events.Value.Unique.Add<LevelFiled>();
            }
        }

        private void OnKernelHeal(ref Command_Kernel_Heal command)
        {
            state.Value.Lives += command.heal;
        }
    }
}