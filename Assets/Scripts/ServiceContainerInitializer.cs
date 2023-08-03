using Leopotam.EcsLite;
using td.features._common;
using td.features.dragNDrop;
using td.features.enemy;
using td.features.fx;
using td.features.goPool;
using td.features.impactEnemy;
using td.features.impactKernel;
using td.features.inputEvents;
using td.features.level;
using td.features.pathFinding;
using td.features.projectile;
using td.features.projectile.explosion;
using td.features.projectile.lightning;
using td.features.shard;
using td.features.state;
using td.features.tower;
using td.features.wave;
using td.features.window;
using UnityEngine;

namespace td
{
    public abstract class ServiceContainerInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Debug.Log("ServiceContainerInitializer");
            
            var state = new State();
            state.SuspendEvents();
            // state.LevelNumber = levelNumber;

            var serviceContainer = new BasicServiceContainer()
                .Finalize()
                // .Add(sharedData)
                .Add<IState>(state)
                .Add(new Common_Pools())
                .Add(new Common_Service())
                // .Add(shardsConfig)
                // .Add(new EcsWorldsStorage(world, outerWorld))
                .Add(new DragNDrop_Pools())
                .Add(new DragNDrop_Service())
                .Add(new Enemy_Converter())
                .Add(new Enemy_Service())
                .Add(new EnemyPath_Service())
                .Add(new Enemy_Pools())
                .Add<IEventBus>(new EventsBus(32, 32, null, LogLevel.Critical))
                .Add(new ImpactKernel_Service())
                .Add(new ImpactEnemy_Service())
                // .Add(new Level_Pools())
                .Add(new Explosion_Converter())
                .Add(new Explosion_Service())
                .Add(new LightningLine_Converter())
                .Add(new LightningLine_Service())
                .Add(new Projectile_Converter())
                .Add(new Projectile_Pools())
                .Add(new Projectile_Service())
                .Add(new Shard_Pools())
                .Add(new Shard_Service())
                .Add(new ShardCalculator())
                .Add(new Shard_Converter())
                .Add(new Tower_Service())
                .Add(new Tower_Converter())
                .Add(new Tower_Pools())
                .Add(new Windows_Service())
                .Add(new GameObjectPool_Service())
                .Add(new Wave_Service())
                .Add(new LevelLoader_Service())
                .Add(new LevelMap_Service())
                .Add(new LevelMap())
                .Add<IPath_Service>(new Path_Service())
                .Add(new Prefab_Service())
                .Add(new MB_Shard_Service())
                .Add(new MB_Tower_Service())
                .Add(new InputEvents_Pools())
                .Add(new InputEvents_Service())
                .Add(new FX_Service())
                .Add(new FX_Pools())
                ;
        }
    }
}