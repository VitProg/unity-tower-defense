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
using td.features.shards;
using td.features.shards.config;
using td.features.shards.ui;
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
        [Required][SerializeField] private EcsUguiEmitter uguiEmitter;
        [Required][SerializeField] private CinemachineVirtualCamera virtualCamera;
        [Required][SerializeField] private HightlightGridByCursor hightlightGridByCursor;
        [Required] [SerializeField] private ShardsConfig shardsConfig;

        [MinValue(1), MaxValue(4)]
        public uint levelNumber;

        private IEcsSystems systems;
        
        [Inject] private LevelMap levelMap;
        [Inject] private LevelState levelState;

        private void Start()
        {
            var sharedData = new SharedData()
            {
                UGUIEmitter = uguiEmitter,
                VirtualCamera = virtualCamera,
                MainCamera = Camera.main,
                HightlightGrid = hightlightGridByCursor
            };

            var world = new EcsWorld();
            var outerWorld = new EcsWorld();
            var uguiWorld = new EcsWorld();

            // converters
            var converters = new EntityConverters();
            converters
                .Add(new EnemyEntityConverter())
                .Add(new ProjectileEntityConverter())
                .Add(new TowerEntityConverter())
                .Add(new ShardEntityConverter())
                ;
            // ---

            systems = new EcsSystems(world, sharedData);

            systems
                .AddWorld(outerWorld, Constants.Worlds.Outer)
                .AddWorld(uguiWorld, Constants.Worlds.UI)

                #region Levels
                .Add(new LoadLevelExecutor())
                .Add(new LevelFinishedHandler())
                .DelHere<LevelFinishedOuterEvent>(Constants.Worlds.Outer)
                #endregion

                .Add(new MoveToTargetSystem())
                .Add(new SmoothRotateExecutor())

                #region Tower
                .Add(new CalcDistanceToKernelSystem())
                .Add(new FindTargetByRadiusSystem())
                
                .Add(new CannonTowerFireSystem())
                
                .Add(new ShardInitSystem())
                .Add(new ShardDragNDropSystem())
                .DelHere<ShardUIDownEvent>()
                .Add(new ShardTowerFireSystem())
                
                .Add(new TowerBuySystem())
                .Add(new TowerShowRadiusSystem())
                #endregion
                
                #region Fire/Projectile
                // .Add(new SpawnProjectileExecuter())
                .Add(new ProjectileTargetCorrectionSystem())
                .Add(new ProjectileReachEnemyHandler())
                // .DelHere<SpawnProjectileOuterCommand>(Constants.Worlds.Outer)
                #endregion
                
                #region Inpacts
                // обработка команды получения урона вррагом
                .Add(new TakeDamageSystem())
                .DelHere<TakeDamageOuter>(Constants.Worlds.Outer)

                // обработка события получение врагом бафа/дебафа
                .Add(new SpeedDebuffSystem())
                .Add(new PoisonDebuffSystem())
                #endregion

                #region Waves
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
                #endregion

                #region L6_Enemies
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
                .DelHere<EnemyReachingKernelEvent>()
                #endregion

                #region Kernel
                .Add(new KernalChangeLivesExecutor())
                .DelHere<KernalDamageOuterCommand>(Constants.Worlds.Outer)
                .DelHere<KernelHealOuterCommand>(Constants.Worlds.Outer)
                #endregion

                #region UI
                .Add(new UpdateUISystem())
                .DelHere<UpdateUIOuterCommand>(Constants.Worlds.Outer)
                #endregion
                
                #region Input
                .DelHere<DragStartEvent>()
                .DelHere<DragEndEvent>()
                .Add(new DragNDropWorldSystem())
                .Add(new DragNDropCameraSystem())
                #endregion

                #region Camera
                .Add(new CameraMoveSystem())
                .Add(new CameraZoomSystem())
                #endregion

                // обработка команды удаления GameObject сщ сцены
                .Add(new RemoveGameObjectExecutor())
                .DelHere<RemoveGameObjectCommand>()
                
                // очистка
                .DelHere<ReachingTargetEvent>()
                

#if UNITY_EDITOR
                .Add(new EcsWorldDebugSystem())
                .Add(new EcsWorldDebugSystem(Constants.Worlds.Outer))
                .Add(new EcsWorldDebugSystem(Constants.Worlds.UI))
#endif
                // .Add(new ConvertSceneSys())
                .Add(new SturtupInitSystem())
                .InjectLite(
                    new LevelState(levelNumber),
                    new LevelMap(),
                    new LevelLoader(),
                    new PathService(),
                    new GameObjectPoolService(),
                    new ProjectileService(),
                    new ShardCalculator(),
                    new EnemyPathService(),
                    shardsConfig,
                    converters
                )
                .Inject()
                .InjectUgui(uguiEmitter, Constants.Worlds.UI)
                .Init();
        }

        private void Update()
        {
            systems?.Run();
        }

        private void OnDestroy()
        {
            systems?.GetWorld(Constants.Worlds.UI).Destroy();
            systems?.GetWorld()?.Destroy();
            systems?.GetWorld(Constants.Worlds.Outer).Destroy();;
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
                DI.GetCustom<LevelState>(),
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

            var foldStyle = EditorStyles.foldoutHeader;
            foldStyle.normal.background = Texture2D.linearGrayTexture;
            levelConfigShowed = EditorGUILayout.Foldout(levelConfigShowed, "LevelConfig.json", foldStyle);

            if (levelConfigShowed)
            {
                // EditorGUILayout.LabelField("Level Json", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextArea(JsonUtility.ToJson(DI.GetCustom<LevelMap>()?.LevelConfig, true));
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndVertical();
        }
    }
#endif
}