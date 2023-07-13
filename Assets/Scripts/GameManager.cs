using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Unity.Ugui;
using Leopotam.EcsLite.UnityEditor;
// using Mitfart.LeoECSLite.UniLeo;
using NaughtyAttributes;
using td.common;
using td.components.commands;
using td.components.events;
using td.features.camera;
using td.features.dragNDrop;
using td.features.enemies;
using td.features.enemies.components;
using td.features.enemies.systems;
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
using td.features.ui;
using td.features.waves;
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
using UnityEngine.Serialization;

namespace td
{
    public class GameManager : MonoBehaviour
    {
        [Required] [SerializeField] private EcsUguiEmitter uguiEmitter;
        [Required] [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Camera mainCamera;
        [Required] [SerializeField] private Camera canvasCamera;
        [Required] [SerializeField] private HightlightGridByCursor hightlightGridByCursor;
        [Required] [SerializeField] private ShardsConfig shardsConfig;

        [Required] public ShardCollectionPanel shardCollectionPanel;
        [Required] public ShardStorePopup shardStorePopup;
        [Required] public ShardInfoPanel shardInfoPanel;
        [Required] public CombineShardCostPopup combineShardCostPopup;
        
        [Required] public ShardMonoBehaviour draggableShard;

        [MinValue(1), MaxValue(4)] public uint levelNumber;

        private IEcsSystems systems;

        private void Start()
        {
            var sharedData = new SharedData()
            {
                uguiEmitter = uguiEmitter,
                virtualCamera = virtualCamera,
                mainCamera = mainCamera ? mainCamera : Camera.main,
                canvasCamera = canvasCamera,
                hightlightGrid = hightlightGridByCursor,
                shardCollection = shardCollectionPanel,
                shardStore = shardStorePopup,
                shardInfo = shardInfoPanel,
                draggableShard = draggableShard,
                combineShardCostPopup = combineShardCostPopup,
            };
            
            combineShardCostPopup.Hide();

            var world = new EcsWorld();
            var outerWorld = new EcsWorld();
            var uguiWorld = new EcsWorld();

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
                .AddWorld(uguiWorld, Constants.Worlds.UI);

            #region Levels
            systems
                .Add(new LoadLevelExecutor())
                .Add(new LevelFinishedHandler())
                .DelHere<LevelFinishedOuterEvent>(Constants.Worlds.Outer);
            #endregion

            systems.Add(new LinearMoveToTargetSystem());
            systems.Add(new SmoothRotateExecutor());

            #region Tower
            systems
                .Add(new CalcDistanceToKernelSystem())
                .Add(new FindTargetByRadiusSystem())
                .Add(new CannonTowerFireSystem())
                .Add(new ShardTowerFireSystem())
                .Add(new TowerBuySystem())
                .Add(new TowerShowRadiusSystem());
            #endregion

            #region Shard
            systems
                .Add(new ShardDragNDropSystem())
                .DelHere<UIShardDownEvent>()
                .Add(new ShardAnimateColorSystem())
                .Add(new InitShardCollectionSystem())
                .Add(new InitShardStoreSystem())
                .Add(new ShardCollectionRemoveHiddenExecutor())
                .Add(new UIRefreshShardCollectionExecutor())
                .Add(new UIRefreshShardStoreExecutor())
                .Add(new UIShardStoreShowHideExecutor())
                .Add(new UIShardStoreLevelChangedHandler())
                .Add(new BuyShardExecutor())
                .DelHere<BuyShardCommand>(Constants.Worlds.Outer);
            #endregion

            #region Fire/Projectile
            systems
                .Add(new ProjectileTargetCorrectionSystem())
                .Add(new ProjectileReachEnemyHandler())
                
                .Add(new LightningLineDamageSystem())
                .Add(new LightningLineNeighborsSystem())
                .Add(new LightningLineCorrectionSystem())
                
                .Add(new ExplosionSystem())
                ;
            #endregion

            #region Inpacts
            systems
                // обработка команды получения урона вррагом
                .Add(new TakeDamageSystem())
                .DelHere<TakeDamageOuter>(Constants.Worlds.Outer)

                // обработка события получение врагом бафа/дебафа
                .Add(new SpeedDebuffSystem())
                .Add(new PoisonDebuffSystem())
                .Add(new ShockingDebuffSystem())
                ;
            #endregion

            #region Waves
            systems
                // отсчет до следующей волны
                .Add(new NextWaveCountdownTimerSystem())

                // ожидания окончания волны (когда все враги выйдут и будут убиты или достигнут ядра
                .Add(new WaitForWaveComliteSystem())

                // обработка события увеличени счетчика волн
                .Add(new IncreaseWaveEcecutor())
                .DelHere<IncreaseWaveOuterCommand>(Constants.Worlds.Outer)

                // обработка события запуска волны
                .Add(new StartWaveExecutor())
                .DelHere<StartWaveOuterCommand>(Constants.Worlds.Outer)
                .Add(new SpawnSequenceSystem())
                .Add(new SpawnSequenceFinishedHandler())
                .DelHere<SpawnSequenceFinishedOuterEvent>(Constants.Worlds.Outer)
                ;
            #endregion

            #region Enemies
            systems
                // обработка команды спавна нового врага
                .Add(new SpawnEnemyExecutor())
                .DelHere<SpawnEnemyOuterCommand>(Constants.Worlds.Outer)

                // обработка события достижения следующей клетки
                .Add(new EnemyReachingCellHandler())

                // обработка события смерти врага
                .Add(new EnemyDiedExecutor())
                .DelHere<EnemyDiedCommand>()

                // обработка события достижения врагом ядра
                .Add(new EnemyReachingKernelEventHandle())
                .DelHere<EnemyReachingKernelEvent>();
            #endregion

            #region Kernel
            systems
                .Add(new KernalChangeLivesExecutor())
                .DelHere<KernalDamageOuterCommand>(Constants.Worlds.Outer)
                .DelHere<KernelHealOuterCommand>(Constants.Worlds.Outer);
            #endregion

            #region UI
            systems
                .Add(new UIUpdateSystem())
                .DelHere<UpdateUIOuterCommand>(Constants.Worlds.Outer);
            #endregion

            #region Drug'n'Drop
            systems
                .DelHere<DragRollbackEvent>()
                .DelHere<DragStartEvent>()
                .DelHere<DragEndEvent>()
                .Add(new DragNDropWorldSystem())
                .Add(new DragNDropCameraSystem());
            #endregion

            #region Camera
            systems
                .Add(new CameraMoveSystem())
                .Add(new CameraZoomSystem());
            #endregion

            // обработка команды удаления GameObject со сцены
            systems
                .Add(new IdleRemoveGameObjectExecutor())
                .Add(new RemoveGameObjectExecutor())
                .DelHere<RemoveGameObjectCommand>();

            // очистка
            systems
                .DelHere<ReachingTargetEvent>()
                .DelHere<StateChangedEvent>(Constants.Worlds.Outer)
                .DelHere<StateChangedExEvent>(Constants.Worlds.Outer)
                .DelHere<LevelLoadedOuterEvent>(Constants.Worlds.Outer)
                ;
                // .DelHere<UIRefreshShardStoreOuterCommand>(Constants.Worlds.Outer)
                // .DelHere<UIRefreshShardCollectionOuterCommand>(Constants.Worlds.Outer)
                // .DelHere<UIShowShardStoreOuterCommand>(Constants.Worlds.Outer)
                // .DelHere<UIHideShardStoreOuterCommand>(Constants.Worlds.Outer);


#if UNITY_EDITOR
            systems
                .Add(new EcsWorldDebugSystem())
                .Add(new EcsWorldDebugSystem(Constants.Worlds.Outer))
                .Add(new EcsWorldDebugSystem(Constants.Worlds.UI));
#endif

            systems.Add(new SturtupInitSystem());

            systems.InjectLite(
                state,
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
                shardsConfig,
                converters
            );

            systems.Inject();
            systems.InjectUgui(uguiEmitter, Constants.Worlds.UI);
            systems.Init();
        }

        private void Update()
        {
            systems?.Run();
        }

        private void OnDestroy()
        {
            systems?.GetWorld(Constants.Worlds.UI).Destroy();
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
                DI.GetCustom<State>(),
                "Level State");
            EditorGUI.EndChangeCheck();

            EditorUtils.HorizontalLine(Color.grey);

            EditorGUI.BeginChangeCheck();
            EditorUtils.RenderAllPropertiesOfObject(
                ref idLevelMap,
                DI.GetCustom<LevelMap>(),
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