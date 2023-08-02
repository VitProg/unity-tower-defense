using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.ExtendedSystems;
using NaughtyAttributes;
using td.features._common;
using td.features._common.costPopup;
using td.features._common.flags;
using td.features._common.systems;
using td.features.camera;
using td.features.dragNDrop;
using td.features.dragNDrop.events;
using td.features.dragNDrop.evets;
using td.features.enemy.systems;
using td.features.gameStatus.systems;
using td.features.impactEnemy.components;
using td.features.impactEnemy.systems;
using td.features.impactKernel;
using td.features.infoPanel;
using td.features.inputEvents;
using td.features.level.systems;
using td.features.projectile.systems;
using td.features.projectile.explosion;
using td.features.projectile.lightning;
using td.features.shard;
using td.features.shard.mb;
using td.features.shard.systems;
using td.features.shardCollection;
using td.features.shardStore;
using td.features.state;
using td.features.tower.systems;
using td.features.towerRadius;
using td.features.wave.systems;
using td.features.window;
using td.features.window.common;
using td.monoBehaviours;
using td.utils.di;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using Leopotam.EcsLite.UnityEditor;
#endif

namespace td
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [Required] [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [Required] [SerializeField] private Camera canvasCamera;
        [Required] [SerializeField] private HightlightGridByCursor hightlightGridByCursor;
        [Required] [SerializeField] private ShardsConfig shardsConfig;

        [FormerlySerializedAs("shardCollectionPanel")] [Required] [SerializeField]  private UI_ShardCollection uiShardCollection;
        [FormerlySerializedAs("shardStorePopup")] [Required] [SerializeField]  private UI_ShardStore uiShardStore;
        [FormerlySerializedAs("uiInformationPanel")] [FormerlySerializedAs("shardInfoPanel")] [Required] [SerializeField]  private UI_InfoPanel uiInfoPanel;
        [FormerlySerializedAs("combineShardCostPopup")] [Required] [SerializeField]  private CostPopup costPopup;
        
        [Required] [SerializeField]  private ShardConrol draggableShard;
        
        [Required] [SerializeField]  private Canvas canvas;
        [Required] [SerializeField]  private FadeInOut fade;

        [MinValue(1), MaxValue(4)] public ushort levelNumber;

        // private readonly WindowsService windowsService = new WindowsService();
        
        private IEcsSystems systems;
        private IServiceContainer serviceContainer;

        //todo move to other place
        public async void ShowSettings()
        {
            var state = serviceContainer.Get<IState>();
            var windowsService = serviceContainer.Get<Windows_Service>();
            
            var lastGameSpeed = state.GameSpeed;
            state.GameSpeed = 0f;
            await windowsService.Open(Windows_Service.Type.SettingsMenu);
            await windowsService.WaitClose(Windows_Service.Type.SettingsMenu);
            state.GameSpeed = lastGameSpeed;
            //todo resume game when settings in closed
        }

        private static float GetDeltaTime() => Time.deltaTime;
        
        private void Awake()
        {
            var sharedData = new SharedData()
            {
                // uguiEmitter = uguiEmitter,
                virtualCamera = virtualCamera,
                mainCamera = mainCamera ? mainCamera : Camera.main,
                canvasCamera = canvasCamera,
                hightlightGrid = hightlightGridByCursor,
                uiShardCollection = uiShardCollection,
                uiShardStore = uiShardStore,
                uiInfo = uiInfoPanel,
                draggableShard = draggableShard,
                costPopup = costPopup,
                canvas = canvas,
                fade = fade,
                EnemiesContainer = GameObject.FindGameObjectWithTag(Constants.Tags.EnemiesContainer),
            };
            
            // combineShardCostPopup.Hide();

            var world = new EcsWorld();
            // var outerWorld = new EcsWorld();

            // var state = new State();
            // state.SuspendEvents();
            // state.LevelNumber = levelNumber;

            serviceContainer = ServiceContainer.GetCurrentContainer();

            serviceContainer.Get<IState>().LevelNumber = levelNumber;
            
            serviceContainer.Add(sharedData);
            // serviceContainer.Add(state);
            serviceContainer.Add(shardsConfig);

            // serviceContainer = new BasicServiceContainer()
            //     .Finalize()
            //     .Add(sharedData)
            //     .Add(state)
            //     .Add(shardsConfig)
            //     .Add(new EcsWorldsStorage(world, outerWorld))
            //     .Add(new CommonPoolStorage())
            //     .Add(new DragNDropPoolStorage())
            //     .Add(new DragNDropService())
            //     .Add(new EnemyEntityConverter())
            //     .Add(new EnemyPathService())
            //     .Add(new EnemyPoolStorage())
            //     .Add(new EventBus())
            //     .Add(new ImpactsEnemyPoolStorage())
            //     .Add(new ImpactsKernelPoolStorage())
            //     .Add(new LevelPoolStorage())
            //     .Add(new ExplosionEntityConverter())
            //     .Add(new ExplosionService())
            //     .Add(new LightningLineEntityConverter())
            //     .Add(new LightningLineService())
            //     .Add(new ProjectileEntityConverter())
            //     .Add(new ProjectilePoolStorage())
            //     .Add(new ProjectileService())
            //     .Add(new ShardCalculator())
            //     .Add(new ShardEntityConverter())
            //     .Add(new ShardPoolStorage())
            //     .Add(new StatePoolStorage())
            //     .Add(new TowerEntityConverter())
            //     .Add(new TowerPoolStorage())
            //     .Add(new WavePoolStorage())
            //     .Add(windowsService)
            //     .Add(new GameObjectPoolService())
            //     .Add(new LevelLoader())
            //     .Add(new LevelMap())
            //     .Add<IPathService>(new PathService())
            //     .Add(new PrefabService());
            
            var eventBus = serviceContainer.Get<IEventBus>();
            
            systems = new EcsSystems(world, serviceContainer);
            
            serviceContainer.Add<IEcsSystems>(systems);
            serviceContainer.Add(new EcsWorldsStorage_Service(world, eventBus.GetEventsWorld()));

            systems
                // .AddWorld(outerWorld, Constants.Worlds.Outer)
                .AddWorld(eventBus, Constants.Worlds.EventBus)
                // .AddWorld(uguiWorld, Constants.Worlds.UI);
                ;

            systems.Add(new GameSimilationStatusSystem());
            
            #region Levels
            systems
                .Add(new Level_LoadingSystem())
                .Add(new Level_FinishedSystem());
                // .DelHere<LevelFinishedOuterEvent>(Constants.Worlds.Outer);
            #endregion
            
            #region INputEvents
            systems.Add(new InputEvents_System());
            #endregion

            // systems.Add(new MovementToTargetJobsSystem());
            systems.Add(new ApplyObjectTransformSystem(1 / 30f, 0f, GetDeltaTime));//, 200000, -1, 10));
            systems.Add(new MovementToTargetSystem(1/30f, 0.016f, GetDeltaTime));
            systems.Add(new SmoothRotateSystem(1/30f, 0.033f, GetDeltaTime));
            
            systems.AddGroup(Constants.EcsSystemGroups.ShardSimulation, false, Constants.Worlds.EventBus,
            #region Shard
                new InitializeShardCollectionSystem(),
                new InitializeShardStoreSystem(),
                // new ShardDragNDropSystem(),
                // /* deprecated */new DelHereSystem<UI_Event_Shard_Down>(world),
                // /* deprecated */new UI_ShardAnimateColorSystem(),
                // /* deprecated */new UI_ShardCollection_InitSystem(),
                // /* deprecated */new UI_ShardStore_InitSystem(),
                // /* deprecated */new UI_ShardCollection_RemoveHiddenSystem(),
                // /* deprecated */new UI_ShardCollection_RefreshSystem(),
                // /* deprecated */new UI_ShardStore_RefreshSystem(),
                // /* deprecated */new UI_ShardStore_VisibleSystem(),
                // /* deprecated */new UI_ShardStore_LevelChangesSystem(),
                // new ShardBuyingSystem(),
                // new DelHereSystem<Command_Shard_Buy>(eventBus.GetEventsWorld()),
                new MB_ShardUpdateAndInit_System(1/20f, 0f, GetDeltaTime),
                new InfoPanel_System()
            #endregion
            );

            systems.AddGroup(Constants.EcsSystemGroups.GameSimulation, false, Constants.Worlds.EventBus,
            #region Tower
                new EnemyCalcDistanceToKernelSystem(1/15f, 0f, GetDeltaTime),
                new TowerFindTargetSystem(1/5f, 0.1f, GetDeltaTime),
                new ShardTowerFireSystem(),
                new TowerRadius_System(),
            // new TowerBuySystem(), //todo
                // new TowerShowRadiusSystem(), // todo
            #endregion

            #region Fire/Projectile
                // new ProjectileTargetCorrectionSystem(1/30f, 0f, GetDeltaTime),
                new ProjectileReachTargetSystem(),
                
                new LightningLineDamageSystem(),
                new LightningLineNeighborsSystem(Constants.WeaponEffects.LightningFindNeighborsTimeRemains, 0f, GetDeltaTime),
                new LightningLineCorrectionSystem(1/30f, 0f, GetDeltaTime),
                
                new ExplosionSystem(),
            #endregion

            #region Inpacts
                // обработка команды получения урона вррагом
                new TakeDamageSystem(),
                new PostRunDelSystem<TakeDamage>(eventBus.GetEventsWorld()),
                // обработка события получение врагом бафа/дебафа
                new SpeedDebuffSystem(),
                new PoisonDebuffSystem(),
                new ShockingDebuffSystem(),
            #endregion

            #region Waves
                // отсчет до следующей волны
                new NextWaveCountdownTimerSystem(1/30f, 0.033f, GetDeltaTime),

                // ожидания окончания волны (когда все враги выйдут и будут убиты или достигнут ядра
                new WaitForWaveCompliteSystem(1/5f, 0.25f, GetDeltaTime),

                // обработка события увеличени счетчика волн
                new IncreaseWaveSystem(),
                // new DelHereSystem<IncreaseWaveOuterCommand>(outerWorld),

                // обработка события запуска волны
                new StartWaveSystem(),
                // new DelHereSystem<StartWaveOuterCommand>(outerWorld),
                new SpawnSequenceSystem(),
                new SpawnSequenceFinishedSystem(),
                // new DelHereSystem<SpawnSequenceFinishedOuterEvent>(outerWorld),
            #endregion

            #region Enemies
                // обработка команды спавна нового врага
                // new SpawnEnemyExecutor(),
                // new DelHereSystem<SpawnEnemyOuterCommand>(outerWorld),

                // обработка события достижения следующей клетки
                new EnemyReachingCellSystem(),

                new EnemyHpChangesSystem(),
                // обработка события смерти врага
                new EnemyDiedService(),
                // new DelHereSystem<EnemyDiedCommand>(world),

                // обработка события достижения врагом ядра
                new EnemyReachKernelSystem(),
                // new DelHereSystem<EnemyReachingKernelEvent>(world),
            
                new EnemyFixAnimationSpeedSystem(),
            #endregion

            #region Kernel
                new KernalChangeLivesSystem()
                // new DelHereSystem<KernalDamageOuterCommand>(outerWorld),
                // new DelHereSystem<KernelHealOuterCommand>(outerWorld)
            );
            #endregion

            #region Drug'n'Drop
            systems
                .AddGroup(Constants.EcsSystemGroups.DragNDrop, true, Constants.Worlds.EventBus,
                    new DelHereSystem<DragRollbackEvent>(world),
                    new DelHereSystem<DragStartEvent>(world),
                    new DelHereSystem<DragEndEvent>(world),
                    new DragNDropSystem()
                    // new DragNDropCameraSystem()
                );
            #endregion

            #region Camera

            systems
                .AddGroup(Constants.EcsSystemGroups.Camera, false, Constants.Worlds.EventBus,
                    new CameraMoveSystem(),
                    new CameraZoomSystem()
                );
            #endregion

            // обработка команды удаления GameObject со сцены
            systems
                .AddGroup(Constants.EcsSystemGroups.RemoveGameObject, true, Constants.Worlds.EventBus,
                    // ToDo new IdleRemoveSystem(),
                    new RemoveSystem()
                    // new PostRunDelSystem<Command_Remove>(world)
                );

            // очистка
            systems
                .DelHere<IsTargetReached>()
                // .DelHere<StateChangedOuterEvent>(Constants.Worlds.Outer)
                // .DelHere<StateChangedExEvent>(Constants.Worlds.Outer)
                // .DelHere<LevelLoadedOuterEvent>(Constants.Worlds.Outer)
                ;


#if UNITY_EDITOR
            systems
                .Add(new EcsWorldDebugSystem())
                // .Add(new EcsWorldDebugSystem(Constants.Worlds.Outer))
                .Add(new EcsWorldDebugSystem(Constants.Worlds.EventBus))
                // .Add(new EcsWorldDebugSystem(Constants.Worlds.UI))
                ;
#endif

            systems.Add(new SturtupInitSystem());

            systems.AddAllEvents(eventBus);
            
            // systems.InjectLite(
            //     state,
            //     // new EventBus(),
            //     // new PrefabService(),
            //     // new LevelMap(),
            //     // new LevelLoader(),
            //     new PathService(),
            //     // new GameObjectPoolService(),
            //     // new ProjectileService(),
            //     // new ShardCalculator(),
            //     // new EnemyPathService(),
            //     // new ImpactsEnemyPoolStorage(),
            //     // new LightningLineService(),
            //     // new ExplosionService(),
            //     // new WindowsService(),
            //     shardsConfig
            //     
            //     // new CommonPoolStorage(),
            //     
            //     // converters
            //     // new TowerEntityConverter(),
            //     // new ShardEntityConverter()
            // );
            // systems.InjectLite(EnemyDI.Get());
            // systems.InjectLite(ProjectileInstaller.Get());
            // systems.InjectLite(ImpactsEnemyDI.Get());
            // systems.InjectLite(DragNDropDI.Get());

            systems.InjectEx(serviceContainer);
            // systems.ResolveMonoBehaviours(serviceContainer);
            
            // systems.InjectUgui(uguiEmitter, Constants.Worlds.UI);
            systems.Init();
        }

        private void Update()
        {
            systems?.Run();
        }

        private void OnDestroy()
        {
            ServiceContainer.GetCurrentContainer().Get<IEventBus>().Destroy();

            // systems?.GetWorld(Constants.Worlds.UI).Destroy();
            systems?.GetWorld()?.Destroy();
            // systems?.GetWorld(Constants.Worlds.EventBus).Destroy();7
            systems?.Destroy();

            systems = null;
        }
    }

// #if UNITY_EDITOR
//     [CustomEditor(typeof(GameManager))]
//     public class GameManagerEditor : Editor
//     {
//         private string idLevelState;
//         private string idLevelMap;
//         private bool levelConfigShowed;
//
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//
//             var state = ServiceContainer.Get<State>();
//             var levelMap = ServiceContainer.Get<LevelMap>();
//
//             EditorGUI.BeginChangeCheck();
//             EditorUtils.RenderAllPropertiesOfObject(
//                 ref idLevelState,
//                 state,
//                 "Level State");
//             EditorGUI.EndChangeCheck();
//
//             EditorUtils.HorizontalLine(Color.grey);
//
//             EditorGUI.BeginChangeCheck();
//             EditorUtils.RenderAllPropertiesOfObject(
//                 ref idLevelMap,
//                 levelMap,
//                 "Level Map");
//             EditorGUI.EndChangeCheck();
//
//             EditorUtils.HorizontalLine(Color.grey);
//
//             EditorGUILayout.BeginVertical();
//             EditorGUILayout.Space();
//             EditorGUILayout.Separator();
//
//             // var foldStyle = EditorStyles.foldoutHeader;
//             // foldStyle.normal.background = Texture2D.linearGrayTexture;
//             // levelConfigShowed = EditorGUILayout.Foldout(levelConfigShowed, "LevelConfig.json", foldStyle);
//             //
//             // if (levelConfigShowed)
//             // {
//             //     // EditorGUILayout.LabelField("Level Json", EditorStyles.boldLabel);
//             //     EditorGUI.BeginDisabledGroup(true);
//             //     EditorGUILayout.TextArea(JsonUtility.ToJson(DI.GetCustom<LevelMap>()?.LevelConfig, true));
//             //     EditorGUI.EndDisabledGroup();
//             // }
//
//             EditorGUILayout.EndVertical();
//         }
//     }
// #endif
}