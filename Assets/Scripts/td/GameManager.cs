using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Unity.Ugui;
using Leopotam.EcsLite.UnityEditor;
using Mitfart.LeoECSLite.UniLeo;
using td.common;
using td.components.commands;
using td.components.events;
using td.features.enemies;
using td.features.enemyImpacts;
using td.features.fire;
using td.features.impactsEnemy;
using td.features.impactsKernel;
using td.features.input;
using td.features.levels;
using td.features.towers;
using td.features.ui;
using td.features.waves;
using td.services;
using td.states;
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
        [SerializeField] private EcsUguiEmitter uguiEmitter;

        public LevelState LevelState { get; private set; }
        public LevelMap LevelMap { get; private set; }

        private EcsWorld world;
        private EcsWorld outerWorld;
        private EcsWorld uguiWorld;

        private EcsSystems systems;
        private SharedData sharedData;

        // [FormerlySerializedAs("LevelNumber")] [SerializeField]
        // public int levelNumber;

        private void Start()
        {
            sharedData = new SharedData();

            world = new EcsWorld();
            outerWorld = new EcsWorld();
            uguiWorld = new EcsWorld();

            systems = new EcsSystems(world, sharedData);

            LevelState = new LevelState(systems, 1);
            LevelMap = new LevelMap(LevelState);

            var levelLoader = new LevelLoader(LevelMap);
            var pathService = new PathService(LevelMap);

            systems
                .AddWorld(outerWorld, Constants.Worlds.Outer)
                .AddWorld(uguiWorld, Constants.Worlds.UI)

                #region Levels
                .Add(new LoadLevelExecutor())
                .DelHere<LevelLoadedOuterEvent>(Constants.Worlds.Outer)
                #endregion

                .Add(new MoveToTargetSystem())
                .Add(new SmoothRotateExecutor())

                #region Tower/Fire

                .Add(new CalcDistanceToKernelSystem())
                .Add(new FindTargetByRadiusSystem())
                .Add(new CannonTowerFireSystem())
                .Add(new ProjectileTargetCorrectionSystem())
                .Add(new ProjectileReachTargetHandler())

                #endregion
                
                #region Inpacts

                // обработка команды получения урона вррагом
                .Add(new TakeDamageExecutor())
                .DelHere<TakeDamageOuterCommand>(Constants.Worlds.Outer)

                // обработка события получение врагом бафа/дебафа
                .Add(new TakeBuffDebuffExecutor())
                .DelHere<TakeDebuffOuterCommand>(Constants.Worlds.Outer)

                #endregion

                #region Waves

                // .DelHere<WaveChangedOuterEvent>(Constants.Worlds.Outer)

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

                // обработка события окончания уровня
                .Add(new LevelFinishedHandler())
                .DelHere<LevelFinishedOuterEvent>(Constants.Worlds.Outer)

                #endregion

                #region Enemies

                // обработка команды спавна нового врага
                .Add(new SpawnEnemyExecutor())
                .DelHere<SpawnEnemyOuterCommand>(Constants.Worlds.Outer)

                // обработка события достижения следующей клетки
                .Add(new EnemyReachingCellHandler())
                // .DelHere<ReachingTargetEvent>(Constants.Worlds.Events)

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

                // .Add(new UpdateUIToolkitSystem())
                .Add(new UpdateUISystem())
                .DelHere<UpdateUIOuterCommand>(Constants.Worlds.Outer)
                .Add(new UIInputSystem())

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
                .Add(new ConvertSceneSys())
                .Add(new SturtupInitSystem())
                .Inject()
                .InjectLite(LevelState, LevelMap, levelLoader, pathService)
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
            systems?.Destroy();
            systems?.GetWorld()?.Destroy();
            outerWorld?.Destroy();
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

            var gm = ((GameManager)target);

            EditorGUI.BeginChangeCheck();
            EditorUtils.RenderAllPropertiesOfObject(
                ref idLevelState,
                gm.LevelState,
                "Level State");
            EditorGUI.EndChangeCheck();

            // EditorUtils.RenderAllPropertiesOfObject(ref idLevelMap, ((GameManager)target).LevelMap,"Level Map");

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
                EditorGUILayout.TextArea(JsonUtility.ToJson(((GameManager)target).LevelMap.LevelConfig, true));
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndVertical();
        }
    }
#endif
}