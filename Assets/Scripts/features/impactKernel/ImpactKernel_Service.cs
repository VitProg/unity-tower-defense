using Leopotam.EcsLite;
using td.features.impactKernel.bus;

namespace td.features.impactKernel
{
    public class ImpactKernel_Service
    {
        private readonly EcsInject<IEventBus> events;

        public void TakeDamage(float damage)
        {
            events.Value.Global.Add<Command_Kernel_Damage>().damage = damage;
        }

        public void Heal(float heal)
        {
            events.Value.Global.Add<Command_Kernel_Heal>().heal = heal;
        }
    }
}