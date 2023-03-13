using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.UnityEditor;
using Mitfart.LeoECSLite.UniLeo;
using td.common;
using td.components.commands;
using td.components.events;
using td.components.waves;
using td.services;
using td.systems;
using td.systems.behaviors;
using td.systems.commands;
using td.systems.events;
using td.systems.init;
using td.systems.waves;
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
                .AddWorld(eventsWorld, Constants.Ecs.EventWorldName)
                .Add(new LoadLevelExecutor())
                .DelHere<LoadLevelCommand>(Constants.Ecs.EventWorldName)
                
                .Add(new LevelInitExecutor())
                .DelHere<LevelInitCommand>(Constants.Ecs.EventWorldName)
                
                .Add(new PathInitExecutor())
                .DelHere<PathInitCommand>(Constants.Ecs.EventWorldName)
                
                .Add(new LevelLoadedHandler())
                .DelHere<LevelLoadedEvent>(Constants.Ecs.EventWorldName)

                .Add(new MoveToTargetSystem())
                .Add(new SmoothRotateExecutor())
                
#region Waves Systems
                .DelHere<WaveChangedEvent>(Constants.Ecs.EventWorldName)
                .Add(new NextWaveCountdownTimerSystem())
                .Add(new WaitForWaveComliteSystem())                
                .Add(new IncreaseWaveHandler())
                .DelHere<IncreaseWaveCommand>(Constants.Ecs.EventWorldName)
                
                .Add(new StartWaveExecutor())
                .DelHere<StartWaveCommand>(Constants.Ecs.EventWorldName)
                
                .Add(new SpawnSequenceSystem())
                
                .Add(new SpawnEnemyExecutor())
                .DelHere<SpawnEnemyCommand>(Constants.Ecs.EventWorldName)
                
                .Add(new SpawnSequenceFinishedHandler())
                .DelHere<SpawnSequenceFinishedEvent>(Constants.Ecs.EventWorldName)
                
                .Add(new LevelFinishedHandler())
                .DelHere<LevelFinishedEvent>(Constants.Ecs.EventWorldName)
                
                .Add(new EnemyReachingCellHandler())
                .DelHere<ReachingTargetEvent>(Constants.Ecs.EventWorldName)
#endregion                
                // 
                
                .Add(new RemoveGameObjectExecutor())
                .DelHere<RemoveGameObjectCommand>()

#if UNITY_EDITOR
                .Add(new EcsWorldDebugSystem())
                .Add(new EcsWorldDebugSystem(Constants.Ecs.EventWorldName))
#endif
                .Add(new ConvertSceneSys())
                .Add(new SturtupInitSystem())
                .Inject(levelData)
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