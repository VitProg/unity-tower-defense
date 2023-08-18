using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.wave.bus;
using td.features.wave.systems;
using td.utils.ecs;

namespace td.features.wave
{
    public class Wave_Module : IProtoModuleWithEvents
    {
        private readonly Func<float> getDeltaTime;
        
        public Wave_Module(Func<float> getDeltaTime)
        {
            // Debug.Log($"{GetType().Name} Init");
            this.getDeltaTime = getDeltaTime;
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddService(new Wave_Service(), true)
                //
                // отсчет до следующей волны
                .AddSystem(new NextWaveCountdownTimerSystem(1/30f, 0.033f, getDeltaTime))
                // ожидания окончания волны (когда все враги выйдут и будут убиты или достигнут ядра
                .AddSystem(new WaitForWaveCompliteSystem(1/5f, 0.25f, getDeltaTime))
                // обработка события увеличени счетчика волн
                .AddSystem(new IncreaseWaveSystem())
                // обработка события запуска волны
                .AddSystem(new StartWaveSystem())
                .AddSystem(new SpawnSequenceSystem())
                .AddSystem(new SpawnSequenceFinishedSystem())
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

        public Type[] Events() => Ev.E<
            Command_Wave_Increase,
            Command_Wave_Start,
            Event_SpawnSequence_Finished,
            Event_Wave_Changed,
            Wait_AllEnemiesAreOver,
            Wave_NextCountdown,
            Wave_SpawnSequence
        >();
    }
}