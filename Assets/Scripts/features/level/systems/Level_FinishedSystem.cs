using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level.bus;
using td.features.state;
using UnityEngine;

namespace td.features.level.systems
{
    public class Level_FinishedSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private EventBus events;

        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
        }

        //------------------------------------------//

        private void OnLevelFinished(ref Event_LevelFinished _)
        {
            var spawnSequenceCount = state.GetActiveSpawnCount();
            var enemiesCount = state.GetEnemiesCount();

            if (
                state.GetWaveNumber() + 1 >= state.GetWaveCount() &&
                spawnSequenceCount <= 0 &&
                enemiesCount <= 0
            )
            {
                Debug.Log("LEVEL COMPLETE!!!");
                //todo show Victory screen
                events.unique.GetOrAdd<Command_LoadLevel>().levelNumber = (ushort)(state.GetLevelNumber() + 1);
            }
        }
    }
}