using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.impactKernel.bus;

namespace td.features.impactKernel
{
    public class ImpactKernel_Service
    {
        [DI] private EventBus events;

        public void TakeDamage(float damage)
        {
            events.global.Add<Command_Kernel_Damage>().damage = damage;
        }

        public void Heal(float heal)
        {
            events.global.Add<Command_Kernel_Heal>().heal = heal;
        }
    }
}