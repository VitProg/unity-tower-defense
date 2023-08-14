using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.fx.effects;
using td.features.impactEnemy.bus;

namespace td.features.fx.listeners
{
    public class FX_GotDebuff_ListenerSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private FX_Service fxService;
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Event_GotPoisonDebuff>(OnGotPoisonDebuff);
            events.global.ListenTo<Event_GotShockingDebuff>(OnGotShockingDebuff);
            events.global.ListenTo<Event_GotSpeedDebuff>(OnGotSpeedDebuff);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Event_GotPoisonDebuff>(OnGotPoisonDebuff);
            events.global.RemoveListener<Event_GotShockingDebuff>(OnGotShockingDebuff);
            events.global.RemoveListener<Event_GotSpeedDebuff>(OnGotSpeedDebuff);
        }
        
        // ---------------------------------------------------

        private void OnGotSpeedDebuff(ref Event_GotSpeedDebuff ev)
        {
            fxService.entityFallow.GetOrAdd<ColdStatusFX>(
                ev.entity,
                ev.duration
            );
        }

        private void OnGotShockingDebuff(ref Event_GotShockingDebuff ev)
        {
            fxService.entityFallow.GetOrAdd<ElectroStatusFX>(
                ev.entity,
                ev.duration
            );
        }

        private void OnGotPoisonDebuff(ref Event_GotPoisonDebuff ev)
        {
            fxService.entityFallow.GetOrAdd<PoisonStatusFX>(
                ev.entity,
                ev.duration
            );
        }
    }
}