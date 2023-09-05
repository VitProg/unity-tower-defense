using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features._common.data;
using td.features.eventBus;
using td.features.state.interfaces;
using td.utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace td.features.wave
{
    [Serializable]
    public class Wave_State : IStateExtension
    {
        [DI] private readonly EventBus events;
        private static Type evType = typeof(Event_Wave_StateChanged);
        private Event_Wave_StateChanged ev;
        
        #region Private fields
        private bool waiting = true;
        private int waveNumber;
        private int waveCount;
        private float nextWaveCountdown;
        private WaveSpawnSequence[] spawners = new WaveSpawnSequence[Constants.Level.MaxSpawns];
        private int spawnersCount;
        private int enemiesCount;
        #endregion

        #region Getters
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool GetWaiting() => waiting;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetWaveNumber() => waveNumber;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetWaveCount() => waveCount;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public float GetNextWaveCountdown() => nextWaveCountdown;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref WaveSpawnSequence GetSpawner(int idx) => ref spawners[idx];
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetSpawnersCount() => spawnersCount;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public int GetEnemiesCount() => enemiesCount;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public bool IsWaveActive() => 
            !GetWaiting() &&
            (enemiesCount > 0);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetActiveSpawnersCount() {
            var count = 0;
            for (var i = 0; i < spawnersCount; i++) {
                count += spawners[i].IsFinished() ? 0 : 1;
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AreAllWavesComplete() => waveNumber >= waveCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFinalWave() => waveNumber == waveCount - 1;
        #endregion
        
        #region Setters
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWaiting(bool value)
        {
            if (waiting == value) return;
            waiting = value;
            ev.waiting = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWaveNumber(int value)
        {
            if (waveNumber == value) return;
            waveNumber = value;
            ev.waveNumber = true;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void IncreaseWaveNumber() => SetWaveNumber(waveNumber + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWaveCount(int value)
        {
            if (waveCount == value) return;
            waveCount = value;
            ev.waveCount = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetNextWaveCountdown(float value)
        {
            if (FloatUtils.IsEquals(nextWaveCountdown, value)) return;
            nextWaveCountdown = value;
            ev.nextWaveCountdown = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReduceNextWaveCountdown(float relative) => SetNextWaveCountdown(nextWaveCountdown - relative);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearSpawners()
        {
            if (spawnersCount == 0) return;
            spawnersCount = 0;
            ev.spawners = true;
            ev.spawnersCount = true;
        }

        public void AddSpawner(ref LevelConfig.WaveSpawnConfig config)
        {
            if (spawnersCount > spawners.Length) throw new Exception($"Cannot add new spawmer. Limit of spammers has been reached ({spawners.Length})!");
            spawners[spawnersCount] = new WaveSpawnSequence
            {
                config = config,
                enemyCounter = 0,
                lastSpawnPoint = 0,
                timeRemains = config.delayBefore,
            };
            spawnersCount++;
            ev.spawners = true;
            ev.spawnersCount = true;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetEnemiesCount(int value)
        {
            if (enemiesCount == value) return;
            enemiesCount = value;
            ev.enemiesCount = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IncreaseEnemiesCount(int relative = 1) => SetEnemiesCount(enemiesCount + relative);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ReduceEnemiesCount(int relative = 1)
        {
            Debug.Log(">>> REDUCE ENEMIES COUNT " + relative + " " + enemiesCount);
            SetEnemiesCount(enemiesCount - relative);
            Debug.Log(">>> ... " + enemiesCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SomeSpawnerHasBeenUpdated() => ev.someSpawnerHasBeenUpdated = true; 
        #endregion


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetEventType() => evType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SendChanges()
        {
            if (ev.IsEmpty()) return false;
            events.unique.GetOrAdd<Event_Wave_StateChanged>() = ev;
            ev = default;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            waiting = true;
            waveNumber = 0;
            waveCount = 0;
            nextWaveCountdown = 0;
            enemiesCount = 0;
            ClearSpawners();
            ev.All();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Refresh() => ev.All();
        
#if UNITY_EDITOR
        public string GetStateName() => "Wave";
        public void DrawStateProperties(VisualElement root)
        {
            EditorUtils.DrawProperty("Waiting", waiting);
            EditorUtils.DrawProperty("Wave Number", waveNumber);
            EditorUtils.DrawProperty("Wave Count", waveCount);
            EditorUtils.DrawProperty("Next wave countdown", nextWaveCountdown);
            EditorUtils.DrawProperty("Enemy Counter", enemiesCount);

            EditorGUILayout.Space();
            
            EditorUtils.DrawProperty("Active Spawners Count", GetActiveSpawnersCount());
            EditorUtils.DrawProperty("All Waves Complete", AreAllWavesComplete());
            EditorUtils.DrawProperty("Is Final Wave", IsFinalWave());
            
            EditorGUILayout.Space();

            if (EditorUtils.FoldoutBegin("wave_spawners", $"Spawners [{spawnersCount}"))
            {
                for (var idx = 0; idx < spawnersCount; idx++)
                {
                    EditorGUILayout.LabelField($"[{idx}]");
                    EditorUtils.DrawProperty("Time Remains", spawners[idx].timeRemains);
                    EditorUtils.DrawProperty("Last Spawner", spawners[idx].lastSpawnPoint);
                    EditorUtils.DrawProperty("Enemy Counter", spawners[idx].enemyCounter);
                    EditorUtils.DrawProperty("Is Finished", spawners[idx].IsFinished());

                    if (EditorUtils.FoldoutBegin($"wave_spawners[{idx}]", $"Config"))
                    {
                        EditorUtils.DrawProperty("Spawner", spawners[idx].config.spawner);
                        EditorUtils.DrawProperty("Select Method", spawners[idx].config.selectMethod.ToString());
                        EditorUtils.DrawProperty("Quantity", spawners[idx].config.quantity);
                        EditorUtils.DrawProperty("Speed", spawners[idx].config.speed);
                        EditorUtils.DrawProperty("Health", spawners[idx].config.health);
                        EditorUtils.DrawProperty("Damage", spawners[idx].config.damage);
                        EditorUtils.DrawProperty("Delay Before", spawners[idx].config.delayBefore);
                        EditorUtils.DrawProperty("Delay Between", spawners[idx].config.delayBetween);
                        ref var cfg = ref spawners[idx].config;
                        EditorUtils.DrawProperty("Scale", cfg.scale != null ? $"[{cfg.scale[0]}, {cfg.scale[1]}" : "");
                        EditorUtils.DrawProperty("Offset", cfg.offset != null ? $"[{cfg.offset[0]}, {cfg.offset[1]}]" : "");

                        if (EditorUtils.FoldoutBegin($"wave_spawners[{idx}].enemies", $"Enemies {cfg.enemies.Length}"))
                        {
                            for (var i = 0; i < cfg.enemies.Length; i++)
                            {
                                EditorGUILayout.LabelField($"[{idx}]");
                                EditorGUI.indentLevel++;
                                {
                                    EditorUtils.DrawProperty("Name", cfg.enemies[i].name);
                                    EditorUtils.DrawProperty("type", cfg.enemies[i].type);
                                    EditorUtils.DrawProperty("Variant", cfg.enemies[i].variant);
                                }
                                EditorGUI.indentLevel--;
                            }
                            EditorUtils.FoldoutEnd();
                        }
                        EditorUtils.FoldoutEnd();
                    }
                    EditorGUILayout.Space(5);
                }
                EditorUtils.FoldoutEnd();
            }
        }
#endif
    }
}