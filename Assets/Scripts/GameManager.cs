using Cysharp.Threading.Tasks;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using NaughtyAttributes;
using td.features._common;
using td.features.building;
using td.features.camera;
using td.features.destroy;
using td.features.ecsConverter;
using td.features.enemy;
using td.features.eventBus;
using td.features.goPool;
using td.features.impactEnemy;
using td.features.impactKernel;
using td.features.infoPanel;
using td.features.inputEvents;
using td.features.level;
using td.features.level.bus;
using td.features.movement;
using td.features.prefab;
using td.features.priceTimePopup;
using td.features.projectile;
using td.features.shard;
using td.features.spriteAnimator;
using td.features.state;
using td.features.tower;
using td.features.wave;
using td.features.window;
using td.utils;
using td.utils.di;
using td.utils.ecs;
using UnityEngine;

namespace td
{
    [DefaultExecutionOrder(-10000)]
    public class GameManager : MonoBehaviour
    {
        [MinValue(1), MaxValue(999)] public ushort levelNumber = 1;

        private ProtoWorld mainWorld;
        private ProtoWorld eventsWorld;
#if !NO_FX
        private ProtoWorld fxWorld;
#endif
        private IProtoSystems systems;
        
        //todo move to other place
        public async UniTaskVoid ShowSettings()
        {
            var state = ServiceContainer.Get<State>();
            var windowsService = ServiceContainer.Get<Window_Service>();
            
            var lastGameSpeed = state.GetGameSpeed();
            state.SetGameSpeed(0f);
            await windowsService.Open(Window_Service.Type.SettingsMenu);
            await windowsService.WaitClose(Window_Service.Type.SettingsMenu);
            state.SetGameSpeed(lastGameSpeed);
            //todo resume game when settin gs in closed
        }

        private static float GetDeltaTime() => Time.deltaTime;
        
        private void Awake()
        { 
            // QualitySettings.vSyncCount = 1;
            // Application.targetFrameRate = 120;
            
            // main
            var stateModule = new State_Module();
            var mainModules = new ProtoModules(
                stateModule,
                new Common_Module(),
                new Destroy_Module(),
                new Movement_Module(GetDeltaTime),
                new InputEvents_Module(),
                new Prefab_Module(),
                new GOPool_Module(),
                new EcsConverter_Module(),
                new Camera_Module(),
                new Level_Module(),
                new PriceTimePopup_Module(),
                new InfoPanel_Module(),
                new SpriteAnimator_Module(), //??
                new Window_Module(), //??
                new Building_Module(),
                new Wave_Module(GetDeltaTime),
                new Projectile_Module(GetDeltaTime),
                new Enemy_Module(GetDeltaTime),
                new Tower_Module(GetDeltaTime),
                new Shard_Module(GetDeltaTime),
                new ImpactEnemy_Module(),
                new ImpactKernel_Module()
            );
            var mainModule = mainModules.BuildModule();
            var mainAspect = mainModules.BuildAspect();
            
            // events
            var eventBusModule = new EventBus_Module();
            var eventsModules = new ProtoModules(
                eventBusModule
            );
            var eventsModule = eventsModules.BuildModule();
            var eventsAspect = eventsModules.BuildAspect();
            
            // fx
#if !NO_FX
            var fxModules = new ProtoModules(
                new features.fx.FX_Module(GetDeltaTime)
            );
            var fxModule = fxModules.BuildModule();
            var fxAspect = fxModules.BuildAspect();
#endif      
            //
            stateModule.AddStateExtensions(mainModules.BuildStateExtensions());
            stateModule.AddStateExtensions(eventsModules.BuildStateExtensions());
#if !NO_FX
            stateModule.AddStateExtensions(fxModules.BuildStateExtensions());
#endif      
            //
            eventBusModule.AddEvents(mainModules.BuildEvents());
            eventBusModule.AddEvents(eventsModules.BuildEvents());
#if !NO_FX
            eventBusModule.AddEvents(fxModules.BuildEvents());
#endif

            // worlds
            mainWorld = new ProtoWorld(mainAspect);
            eventsWorld = new ProtoWorld(eventsAspect);
#if !NO_FX
            fxWorld = new ProtoWorld(fxAspect);
#endif
            
            systems = new ProtoSystems(mainWorld);
            systems
                .AddModule(new TotalAutoInjectModule())
                //
                .AddWorld(eventsWorld, Constants.Worlds.EventBus)
#if !NO_FX
                .AddWorld(fxWorld, Constants.Worlds.FX)
#endif
                //
                .AddModule(mainModule)
                .AddModule(eventsModule)
#if !NO_FX
                .AddModule(fxModule)
#endif
#if UNITY_EDITOR
                .AddModule(new UnityModule())
#endif
                .AddPoint(Constants.EcsPoints.FX)
                ;

            systems.AddSystem(new SturtupInitSystem());

            // ServiceContainer.Get<EventBus>().unique.GetOrAdd<Command_LoadLevel>().levelNumber = levelNumber;
            // ServiceContainer.Get<State>().SetLevelNumber(levelNumber);
            
            systems.Init();
        }

        private void Start()
        {
            ServiceContainer.Get<Level_State>().SetLevelNumber(levelNumber);
            ServiceContainer.Get<State>().SendChanges();
        }

        private void Update()
        {
            systems.Run();
        }

        private void OnDestroy()
        {
            if (systems != null) {
                systems.Destroy ();
                systems = null;
            }
            if (mainWorld != null) {
                mainWorld.Destroy ();
                mainWorld = null;
            }
#if FX
            if (fxWorld != null) {
                fxWorld.Destroy ();
                fxWorld = null;
            }
#endif      
            if (eventsWorld != null) {
                eventsWorld.Destroy ();
                eventsWorld = null;
            }
        }
    }
    
    public class SturtupInitSystem : IProtoPreInitSystem
    {
        public void PreInit(IProtoSystems systems)
        {
            ServiceContainer.Get<Window_Service>().Open(Window_Service.Type.MainMenu, true).Forget();
        }
    }
}