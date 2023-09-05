using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level.bus;
using td.features.tower.mb;
using UnityEngine;

namespace td.features.tower.systems
{
    public class Tower_InitOnLevelStart_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private Tower_Aspect aspect;
        [DI] private EventBus events;
        [DI] private Tower_Converter towerConverter;
        
        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_LevelLoaded>(OnLevelLoaded);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Event_LevelLoaded>(OnLevelLoaded);
        }
        
        // ----------------------------------------------------------------
        
        private void OnLevelLoaded(ref Event_LevelLoaded ev)
        {
            foreach (var shardTowerMb in Object.FindObjectsOfType<ShardTowerMonoBehaviour>())
            {
                var towerEntity = towerConverter.GetEntity(shardTowerMb.gameObject) ?? aspect.World().NewEntity();
                towerConverter.Convert(shardTowerMb.gameObject, towerEntity);
            }
        }

    }
}