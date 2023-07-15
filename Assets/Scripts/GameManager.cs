using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.UnityEditor;
using NaughtyAttributes;
using td.common;
using td.components.commands;
using td.components.events;
using td.features.camera;
using td.features.dragNDrop;
using td.features.enemies;
using td.features.enemies.components;
using td.features.enemies.systems;
using td.features.eventBus;
using td.features.impactsEnemy;
using td.features.impactsKernel;
using td.features.levels;
using td.features.projectiles;
using td.features.projectiles.explosion;
using td.features.projectiles.lightning;
using td.features.shards;
using td.features.shards.commands;
using td.features.shards.config;
using td.features.shards.events;
using td.features.shards.executors;
using td.features.shards.init;
using td.features.shards.mb;
using td.features.state;
using td.features.towers;
using td.features.waves;
using td.features.windows;
using td.features.windows.common;
using td.monoBehaviours;
using td.services;
using td.services.ecsConverter;
using td.systems.behaviors;
using td.systems.commands;
using td.systems.init;
using td.utils;
using td.utils.ecs;
using UnityEditor;
using UnityEngine;

namespace td
{
    public class GameManager : MonoBehaviour
    {
        // [Required] [SerializeField] private EcsUguiEmitter uguiEmitter;
        [Required] [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Camera mainCamera;
        [Required] [SerializeField] private Camera canvasCamera;
        [Required] [SerializeField] private HightlightGridByCursor hightlightGridByCursor;
        [Required] [SerializeField] private ShardsConfig shardsConfig;

        [Required] [SerializeField]  private ShardCollectionPanel shardCollectionPanel;
        [Required] [SerializeField]  private ShardStorePopup shardStorePopup;
        [Required] [SerializeField]  private ShardInfoPanel shardInfoPanel;
        [Required] [SerializeField]  private CombineShardCostPopup combineShardCostPopup;
        
        [Required] [SerializeField]  private ShardMonoBehaviour draggableShard;
        
        [Required] [SerializeField]  private Canvas canvas;
        [Required] [SerializeField]  private FadeInOut fade;

        [MinValue(1), MaxValue(4)] public uint levelNumber;

        private IEcsSystems systems;
        
        //todo move to other place
        public async void ShowSettings()
        {
            var windowsService = DI.Get<WindowsService>()!;
            var state = DI.Get<State>()!;
            var lastGameSpeed = state.GameSpeed;
            state.GameSpeed = 0f;
            await windowsService.Open(WindowsService.Type.SettingsMenu);
            await windowsService.WaitClose(WindowsService.Type.SettingsMenu);
            state.GameSpeed = lastGameSpeed;

            //todo resume game when settings in closed
        }
        
        private void Start()
        {
            var sharedData = new SharedData()
            {
                // uguiEmitter = uguiEmitter,
                virtualCamera = virtualCamera,
                mainCamera = mainCamera ? mainCamera : Camera.main,
                canvasCamera = canvasCamera,
                hightlightGrid = hightlightGridByCursor,
                shardCollection = shardCollectionPanel,
                shardStore = shardStorePopup,
                shardInfo = shardInfoPanel,
                draggableShard = draggableShard,
                combineShardCostPopup = combineShardCostPopup,
                canvas = canvas,
                fade = fade,
            };
            
            combineShardCostPopup.Hide();

            var world = new EcsWorld();
            var outerWorld = new EcsWorld();
            // var uguiWorld = new EcsWorld();

            var state = new State();
            state.SuspendEvents();
            state.LevelNumber = levelNumber;

            // converters
            var converters = new EntityConverters();
            converters
                .Add(new EnemyEntityConverter())
                .Add(new ProjectileEntityConverter())
                .Add(new LightningLineEntityConverter())
                .Add(new ExplosionEntityConverter())
                .Add(new TowerEntityConverter())
                .Add(new ShardEntityConverter())
                ;
            // ---
            
            systems = new EcsSystems(world, sharedData);

            systems
                .AddWorld(outerWorld, Constants.Worlds.Outer)
                // .AddWorld(uguiWorld, Constants.Worlds.UI);
                ;

            #region Levels
            systems
                .Add(new LoadLevelExecutor())
                .Add(new LevelFinishedHandler())
                .DelHere<LevelFinishedOuterEvent>(Constants.Worlds.Outer);
            #endregion

            systems.Add(new LinearMoveToTargetSystem());
            systems.Add(new SmoothRotateExecutor());
            
            systems.AddGroup(Constants.EcsSystemGroups.ShardSimulation, false, Constants.Worlds.Outer,
            #region Shard
                new ShardDragNDropSystem(),
                new DelHereSystem<UIShardDownEvent>(world),
                new ShardAnimateColorSystem(),
                new InitShardCollectionSystem(),
                new InitShardStoreSystem(),
                new ShardCollectionRemoveHiddenExecutor(),
                new UIRefreshShardCollectionExecutor(),
                new UIRefreshShardStoreExecutor(),
                new UIShardStoreShowHideExecutor(),
                new UIShardStoreLevelChangedHandler(),
                new BuyShardExecutor(),
                new DelHereSystem<BuyShardCommand>(outerWorld)
            #endregion
            );

            systems.AddGroup(Constants.EcsSystemGroups.GameSimulation, false, Constants.Worlds.Outer,
            #region Tower
                new CalcDistanceToKernelSystem(),
                new FindTargetByRadiusSystem(),
                new CannonTowerFireSystem(),
                new ShardTowerFireSystem(),
                new TowerBuySystem(),
                new TowerShowRadiusSystem(),
            #endregion

            #region Fire/Projectile
                new ProjectileTargetCorrectionSystem(),
                new ProjectileReachEnemyHandler(),
                
                new LightningLineDamageSystem(),
                new LightningLineNeighborsSystem(),
                new LightningLineCorrectionSystem(),
                
                new ExplosionSystem(),
            #endregion

            #region Inpacts
                // обработка команды получения урона вррагом
                new TakeDamageSystem(),
                new DelHereSystem<TakeDamageOuter>(outerWorld),

                // обработка события получение врагом бафа/дебафа
                new SpeedDebuffSystem(),
                new PoisonDebuffSystem(),
                new ShockingDebuffSystem(),
            #endregion

            #region Waves
                // отсчет до следующей волны
                new NextWaveCountdownTimerSystem(),

                // ожидания окончания волны (когда все враги выйдут и будут убиты или достигнут ядра
                new WaitForWaveCompliteSystem(),

                // обработка события увеличени счетчика волн
                new IncreaseWaveEcecutor(),
                new DelHereSystem<IncreaseWaveOuterCommand>(outerWorld),

                // обработка события запуска волны
                new StartWaveExecutor(),
                new DelHereSystem<StartWaveOuterCommand>(outerWorld),
                new SpawnSequenceSystem(),
                new SpawnSequenceFinishedHandler(),
                new DelHereSystem<SpawnSequenceFinishedOuterEvent>(outerWorld),
            #endregion

            #region Enemies
                // обработка команды спавна нового врага
                new SpawnEnemyExecutor(),
                new DelHereSystem<SpawnEnemyOuterCommand>(outerWorld),

                // обработка события достижения следующей клетки
                new EnemyReachingCellHandler(),

                // обработка события смерти врага
                new EnemyDiedExecutor(),
                new DelHereSystem<EnemyDiedCommand>(world),

                // обработка события достижения врагом ядра
                new EnemyReachingKernelEventHandle(),
                new DelHereSystem<EnemyReachingKernelEvent>(world),
            #endregion

            #region Kernel
                new KernalChangeLivesExecutor(),
                new DelHereSystem<KernalDamageOuterCommand>(outerWorld),
                new DelHereSystem<KernelHealOuterCommand>(outerWorld)
            );
            #endregion

            #region Drug'n'Drop
            systems
                .AddGroup(Constants.EcsSystemGroups.DragNDrop, true, Constants.Worlds.Outer,
                    new DelHereSystem<DragRollbackEvent>(world),
                    new DelHereSystem<DragStartEvent>(world),
                    new DelHereSystem<DragEndEvent>(world),
                    new DragNDropWorldSystem(),
                    new DragNDropCameraSystem()
                );
            #endregion

            #region Camera

            systems
                .AddGroup(Constants.EcsSystemGroups.Camera, false, Constants.Worlds.Outer,
                    new CameraMoveSystem(),
                    new CameraZoomSystem()
                );
            #endregion

            // обработка команды удаления GameObject со сцены
            systems
                .AddGroup(Constants.EcsSystemGroups.RemoveGameObject, true, Constants.Worlds.Outer,
                    new IdleRemoveGameObjectExecutor(),
                    new RemoveGameObjectExecutor(),
                    new DelHereSystem<RemoveGameObjectCommand>(world)
                );

            // очистка
            systems
                .DelHere<ReachingTargetEvent>()
                .DelHere<StateChangedOuterEvent>(Constants.Worlds.Outer)
                // .DelHere<StateChangedExEvent>(Constants.Worlds.Outer)
                .DelHere<LevelLoadedOuterEvent>(Constants.Worlds.Outer)
                ;


#if UNITY_EDITOR
            systems
                .Add(new EcsWorldDebugSystem())
                .Add(new EcsWorldDebugSystem(Constants.Worlds.Outer))
                // .Add(new EcsWorldDebugSystem(Constants.Worlds.UI))
                ;
#endif

            systems.Add(new SturtupInitSystem());

            systems.InjectLite(
                state,
                new EventBus(),
                new PrefabService(),
                new LevelMap(),
                new LevelLoader(),
                new PathService(),
                new GameObjectPoolService(),
                new ProjectileService(),
                new ShardCalculator(),
                new EnemyPathService(),
                new LightningLineService(),
                new ExplosionService(),
                new WindowsService(),
                shardsConfig,
                converters
            );

            systems.Inject();
            // systems.InjectUgui(uguiEmitter, Constants.Worlds.UI);
            systems.Init();
        }

        private void Update()
        {
            systems?.Run();
        }

        private void OnDestroy()
        {
            // systems?.GetWorld(Constants.Worlds.UI).Destroy();
            systems?.GetWorld()?.Destroy();
            systems?.GetWorld(Constants.Worlds.Outer).Destroy();
            ;
            systems?.Destroy();
            systems = null;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        private string idLevelState;
        private string idLevelMap;
        private bool levelConfigShowed;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            EditorUtils.RenderAllPropertiesOfObject(
                ref idLevelState,
                DI.Get<State>(),
                "Level State");
            EditorGUI.EndChangeCheck();

            EditorUtils.HorizontalLine(Color.grey);

            EditorGUI.BeginChangeCheck();
            EditorUtils.RenderAllPropertiesOfObject(
                ref idLevelMap,
                DI.Get<LevelMap>(),
                "Level Map");
            EditorGUI.EndChangeCheck();

            EditorUtils.HorizontalLine(Color.grey);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Separator();

            // var foldStyle = EditorStyles.foldoutHeader;
            // foldStyle.normal.background = Texture2D.linearGrayTexture;
            // levelConfigShowed = EditorGUILayout.Foldout(levelConfigShowed, "LevelConfig.json", foldStyle);
            //
            // if (levelConfigShowed)
            // {
            //     // EditorGUILayout.LabelField("Level Json", EditorStyles.boldLabel);
            //     EditorGUI.BeginDisabledGroup(true);
            //     EditorGUILayout.TextArea(JsonUtility.ToJson(DI.GetCustom<LevelMap>()?.LevelConfig, true));
            //     EditorGUI.EndDisabledGroup();
            // }

            EditorGUILayout.EndVertical();
        }
    }
#endif
}