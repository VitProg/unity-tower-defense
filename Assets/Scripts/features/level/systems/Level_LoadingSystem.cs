using Cysharp.Threading.Tasks;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level.bus;
using td.features.state;
using UnityEngine;

namespace td.features.level.systems
{
    public class Level_LoadingSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private Level_State levelState;
        [DI] private LevelLoader_Service levelLoader;
        [DI] private EventBus events;
        
        private ProtoWorld world;

        public void Init(IProtoSystems systems)
        {
            world = systems.World();
            events.unique.ListenTo<Command_LoadLevel>(OnLoadLevelCommand);
            // events.unique.ListenTo<Event_LevelLoaded>(OnLevelLoaded);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Command_LoadLevel>(OnLoadLevelCommand);
            // events.unique.RemoveListener<Event_LevelLoaded>(OnLevelLoaded);
        }
        
        //------------------------------------------//

        private async void LoadLevel(int levelNumber)
        {
            levelState.SetLevelNumber(levelNumber);

            if (levelLoader.HasLevel())
            {
                levelLoader.LoadLevel(levelNumber);
                
                state.Refresh();
                
                Debug.Log(">>> Event_LevelPreLoaded");
                events.unique.GetOrAdd<Event_LevelPreLoaded>();
                
                await UniTask.Yield();
                await UniTask.Delay(30);
                await UniTask.Yield();

                Debug.Log(">>> Event_LevelLoaded");
                events.unique.GetOrAdd<Event_LevelLoaded>();
            }
            else
            {
                Debug.Log("ALL LEVELS ARE FINISHED!");
            }
        }
        
        private void OnLoadLevelCommand(ref Command_LoadLevel command)
        {
            LoadLevel(command.levelNumber > 0 
                ? command.levelNumber
                : levelState.GetLevelNumber()
            );
        }
    }
}