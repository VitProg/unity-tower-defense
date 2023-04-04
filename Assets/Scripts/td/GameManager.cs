using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
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
using td.features.levels;
using td.features.waves;
using td.services;
using td.systems;
using td.systems.behaviors;
using td.systems.commands;
using td.systems.init;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace td
{
    public class GameManager : MonoBehaviour
    {
        private static EcsWorld world;
        private static EcsWorld eventsWorld;
        private EcsSystems systems;
        private SharedData sharedData;

        [FormerlySerializedAs("LevelNumber")] [SerializeField]
        public int levelNumber;

        private void Start()
        {
            levelData = new LevelData();
            sharedData = new SharedData();

            world = new EcsWorld();
            eventsWorld = new EcsWorld();

            systems = new EcsSystems(world, sharedData);
            systems
                .AddWorld(eventsWorld, Constants.Ecs.EventsWorldName)
                
#region Levels
                .Add(new LoadLevelExecutor())
                .DelHere<LoadLevelCommand>(Constants.Ecs.EventsWorldName)
                
                .Add(new LevelInitExecutor())
                .DelHere<LevelInitCommand>(Constants.Ecs.EventsWorldName)
                
                .Add(new PathInitExecutor())
                .DelHere<PathInitCommand>(Constants.Ecs.EventsWorldName)
                
                .Add(new LevelLoadedHandler())
                .DelHere<LevelLoadedEvent>(Constants.Ecs.EventsWorldName)
                
                .Add(new BuildingsInitSystem())
#endregion

                .Add(new MoveToTargetSystem())
                .Add(new SmoothRotateExecutor())

#region Fire
                .Add(new CalcDistanceToKernelSystem())
                .Add(new FindTargetByRadiusSystem())

                .Add(new CannonTowerFireSystem())
                .Add(new RocketTowerFireSystem())
                
                .Add(new ProjectileTargetCorrectionSystem())
                .Add(new ProjectileReachTargetHandler())
#endregion

#region Inpacts 
                // обработка команды получения урона вррагом
                .Add(new TakeDamageExecutor())
                .DelHere<TakeDamageCommand>(Constants.Ecs.EventsWorldName)
                
                // обработка события получение врагом бафа/дебафа
                .Add(new TakeBuffDebuffExecutor())
                .DelHere<TakeBuffDebuffCommand>(Constants.Ecs.EventsWorldName)
#endregion
                
#region Waves
                .DelHere<WaveChangedEvent>(Constants.Ecs.EventsWorldName)
                
                // отсчет до следующей волны
                .Add(new NextWaveCountdownTimerSystem())
                
                // ожидания окончания волны (когда все враги выйдут и будут убиты или достигнут ядра
                .Add(new WaitForWaveComliteSystem())
                
                // обработка события увеличени счетчика волн
                .Add(new IncreaseWaveEcecutor())
                .DelHere<IncreaseWaveCommand>(Constants.Ecs.EventsWorldName)
                
                // обработка события запуска волны
                .Add(new StartWaveExecutor())
                .DelHere<StartWaveCommand>(Constants.Ecs.EventsWorldName)
                
                .Add(new SpawnSequenceSystem())
                
                
                .Add(new SpawnSequenceFinishedHandler())
                .DelHere<SpawnSequenceFinishedEvent>(Constants.Ecs.EventsWorldName)
                
                // обработка события окончания уровня
                .Add(new LevelFinishedHandler())
                .DelHere<LevelFinishedEvent>(Constants.Ecs.EventsWorldName)
#endregion
                
#region Enemies
                // обработка команды спавна нового врага
                .Add(new SpawnEnemyExecutor())
                .DelHere<SpawnEnemyCommand>(Constants.Ecs.EventsWorldName)
                
                // обработка события достижения следующей клетки
                .Add(new EnemyReachingCellHandler())
                .DelHere<ReachingTargetEvent>(Constants.Ecs.EventsWorldName)
                
                // обработка события смерти врага
                .Add(new EnemyDiedExecutor())
                .DelHere<EnemyDiedCommand>(Constants.Ecs.EventsWorldName)
                
                // обработка события достижения врагом ядра
                .Add(new EnemyReachingKernelEventHandle())
                .DelHere<EnemyReachingKernelEvent>(Constants.Ecs.EventsWorldName)
#endregion

#region Kernel
                .Add(new KernalChangeLivesExecutor())
                .DelHere<KernalDamageCommand>(Constants.Ecs.EventsWorldName)
                .DelHere<KernelHealCommand>(Constants.Ecs.EventsWorldName)
#endregion

                // обработка команды удаления GameObject сщ сцены
                .Add(new RemoveGameObjectExecutor())
                .DelHere<RemoveGameObjectCommand>()

#if UNITY_EDITOR
                .Add(new EcsWorldDebugSystem())
                .Add(new EcsWorldDebugSystem(Constants.Ecs.EventsWorldName))
#endif
                .Add(new ConvertSceneSys())
                .Add(new SturtupInitSystem())
                .Inject(levelData)
                .Inject(new UI())
                .Init();
        }

        [SerializeField] public GameObject enemyPrefab;

        [SerializeField] private LevelData levelData;

        private void Update()
        {
            systems?.Run();
        }

        private void OnDestroy()
        {
            systems?.Destroy();
            systems?.GetWorld()?.Destroy();
            eventsWorld?.Destroy();
            systems = null;
        }
    }
}