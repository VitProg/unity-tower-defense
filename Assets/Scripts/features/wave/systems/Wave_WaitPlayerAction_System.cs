using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.shard.bus;
using td.features.tower.bus;
using td.features.wave.bus;
using UnityEngine;

namespace td.features.wave.systems {
    public class Wave_WaitPlayerAction_System : IProtoInitSystem, IProtoDestroySystem {
        [DI] private EventBus events;
        [DI] private Wave_State waveState;

        public void Init(IProtoSystems systems) {
            events.global.ListenTo<Event_ShardsCombined>(OnShardsCombined);
            events.global.ListenTo<Event_ShardInserted_InBuilding>(OnShardInsertedInBuilding);
            events.global.ListenTo<Event_ShardDropped_OnMap>(OnShardDroppedOnMap);
            events.global.ListenTo<Event_Tower_Created>(OnTowerCreated);
            events.unique.ListenTo<Command_StartNextWave>(OnStartNextWave);
        }

        public void Destroy() {
            events.global.RemoveListener<Event_ShardsCombined>(OnShardsCombined);
            events.global.RemoveListener<Event_ShardInserted_InBuilding>(OnShardInsertedInBuilding);
            events.global.RemoveListener<Event_ShardDropped_OnMap>(OnShardDroppedOnMap);
            events.global.RemoveListener<Event_Tower_Created>(OnTowerCreated);
            events.unique.RemoveListener<Command_StartNextWave>(OnStartNextWave);
        }

        // ----------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void OnShardsCombined(ref Event_ShardsCombined ev) {
            Debug.Log("Wave Wait OnShardsCombined");
            Start();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void OnTowerCreated(ref Event_Tower_Created ev) {
            Debug.Log("Wave Wait OnTowerCreated");
            Start();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void OnShardDroppedOnMap(ref Event_ShardDropped_OnMap obj) {
            Debug.Log("Wave Wait OnShardDroppedOnMap");
            Start();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void OnShardInsertedInBuilding(ref Event_ShardInserted_InBuilding obj) {
            Debug.Log("Wave Wait OnShardInsertedInBuilding");
            Start();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void OnStartNextWave(ref Command_StartNextWave obj) {
            Debug.Log("Wave Wait OnStartNextWave");
            if (waveState.IsWaiting() || waveState.GetNextWaveCountdown() > 0f) {
                waveState.SetNextWaveCountdown(0f);
                waveState.SetWaiting(false);
            }
        }

        // ----------------------------------------------------------------

        private void Start() {
            if (waveState.IsWaiting()) {
                waveState.SetWaiting(false);
            }
        }
    }
}
