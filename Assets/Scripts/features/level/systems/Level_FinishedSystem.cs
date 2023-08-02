using Leopotam.EcsLite;
using td.features.level.bus;
using td.features.state;
using UnityEngine;

namespace td.features.level.systems
{
    public class Level_FinishedSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;

        public void Init(IEcsSystems systems)
        {
            events.Value.Unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
        }

        //------------------------------------------//

        private void OnLevelFinished(ref Event_LevelFinished _)
        {
            var spawnSequenceCount = state.Value.ActiveSpawnCount;
            var enemiesCount = state.Value.EnemiesCount;

            if (
                state.Value.WaveNumber + 1 >= state.Value.WaveCount &&
                spawnSequenceCount <= 0 &&
                enemiesCount <= 0
            )
            {
                Debug.Log("LEVEL COMPLETE!!!");
                //todo show Victory screen
                events.Value.Unique.Add<Command_LoadLevel>().levelNumber = (ushort)(state.Value.LevelNumber + 1);
            }
        }
    }
}