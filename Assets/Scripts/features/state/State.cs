#if  UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
#endif
using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.features.eventBus;
using td.features.state.bus;
using td.features.state.interfaces;
using td.utils;

namespace td.features.state
{
    public class State
    {
        [DI] private readonly State_Aspect aspect;
        [DI] private readonly EventBus events;
        
        private Event_StateChanged ev;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public T Ex<T>() where T : IStateExtension 
        {
            var type = typeof(T);
            if (!aspect.HasEx(type, out var idx))
            {
#if UNITY_EDITOR
                throw new Exception($"State extension {EditorExtensions.GetCleanTypeName(type)} not registered");
#endif
            }
            return (T)aspect.GetExByIndex(idx);
        }
        
        // -----

        #region Privite Fields
        private bool simulationEnabled;
        private float maxLives;
        private float lives;
        //private ushort levelNumber;
        private uint energy;
        private uint maxEnergy;
        // private float nextWaveCountdown;
        // private int waveNumber;
        // private int waveCount;
        // private int activeSpawnCount;
        // private int enemiesCount;
        private int killsCount;
        private float gameSpeed;
        private float lastLives;
        private uint lastEnergy;
        #endregion        

        #region Getters
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public bool GetSimulationEnabled() => simulationEnabled;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsSimulationRunning() => simulationEnabled;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsSimulationSuspended() => !simulationEnabled;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public float GetMaxLives() => maxLives;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public float GetLastLives() => lastLives;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public float GetLives() => lives;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public bool IsDead() => lives == 0;
        // [MethodImpl (MethodImplOptions.AggressiveInlining)] public ushort GetLevelNumber() => levelNumber;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public uint GetLastEnergy() => lastEnergy;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public uint GetEnergy() => energy;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public uint GetMaxEnergy() => maxEnergy;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public bool IsEnoughEnergy(uint price) => energy >= price;
        // [MethodImpl (MethodImplOptions.AggressiveInlining)] public float GetNextWaveCountdown() => nextWaveCountdown;
        // [MethodImpl (MethodImplOptions.AggressiveInlining)] public int GetWaveNumber() => waveNumber;
        // [MethodImpl (MethodImplOptions.AggressiveInlining)] public int GetWaveCount() => waveCount;
        // [MethodImpl (MethodImplOptions.AggressiveInlining)] public int GetActiveSpawnCount() => activeSpawnCount;
        // [MethodImpl (MethodImplOptions.AggressiveInlining)] public int GetEnemiesCount() => enemiesCount;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public float GetGameSpeed() => gameSpeed;
        #endregion

        #region Setters
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetSimulationEnabled(bool value)
        {
            if (simulationEnabled == value) return;
            simulationEnabled = value;
            ev.simulationEnabled = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResumeSimulation() => SetSimulationEnabled(true);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SuspendSimulation() => SetSimulationEnabled(false);
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetMaxLives(float value)
        {
            if (FloatUtils.IsEquals(maxLives, value)) return;
            maxLives = Math.Max(1, value);
            ev.maxLives = true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void SetLastLives(float value) => lastLives = value;
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetLives(float value)
        {
            var v = Math.Clamp(value, 0, maxLives);
            if (FloatUtils.IsEquals(lives, v)) return;
            SetLastLives(lives);
            lives = v;
            ev.lives = true;
        }
     
        /*[MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetLevelNumber(ushort value)
        {
            if (levelNumber == value) return;
            levelNumber = value;
            ev.levelNumber = true;
        }*/
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void SetLastEnergy(uint value) => lastEnergy = value;
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetEnergy(uint value)
        {
            var v = Math.Clamp(value, 0, maxEnergy);
            if (energy == v) return;
            SetLastEnergy(energy);
            energy = v;
            ev.energy = true;
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void IncreaseEnergy(uint relative = 1) => SetEnergy(energy + relative);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReduceEnergy(uint relative) => SetEnergy(energy - relative);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetMaxEnergy(uint value)
        {
            if (maxEnergy == value) return;
            maxEnergy = Math.Max(value, 0);
            ev.maxEnergy = true;

            if (energy < maxEnergy) {
                SetEnergy(maxEnergy);
            }
        }
        
        
        // [MethodImpl (MethodImplOptions.AggressiveInlining)]
        // public void SetNextWaveCountdown(float value)
        // {
        //     if (FloatUtils.IsEquals(nextWaveCountdown, value)) return;
        //     nextWaveCountdown = value;
        //     ev.nextWaveCountdown = true;
        // }
        //
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public void ReduceNextWaveCountdown(float relative) => SetNextWaveCountdown(nextWaveCountdown - relative); 
        //
        // [MethodImpl (MethodImplOptions.AggressiveInlining)]
        // public void SetWaveNumber(int value)
        // {
        //     if (waveNumber == value) return;
        //     waveNumber = value;
        //     ev.waveNumber = true;
        // }
        //
        // [MethodImpl (MethodImplOptions.AggressiveInlining)]
        // public void SetWaveCount(int value)
        // {
        //     if (waveCount == value) return;
        //     waveCount = value;
        //     ev.waveCount = true;
        // }
        //
        // [MethodImpl (MethodImplOptions.AggressiveInlining)]
        // public void SetActiveSpawnCount(int value)
        // {
        //     if (activeSpawnCount == value) return;
        //     activeSpawnCount = value;
        //     ev.activeSpawnCount = true;
        // }
        //
        // [MethodImpl (MethodImplOptions.AggressiveInlining)]
        // public void SetEnemiesCount(int value)
        // {
        //     if (enemiesCount == value) return;
        //     enemiesCount = value;
        //     ev.enemiesCount = true;
        // }
        //
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public void IncreaseEnemiesCount(int relative = 1) => SetEnemiesCount(enemiesCount + relative);
        // [MethodImpl (MethodImplOptions.AggressiveInlining)]
        // public void ReduceEnemiesCount(int relative = 1) => SetEnemiesCount(enemiesCount - relative);
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetKillsCount(int value)
        {
            if (killsCount == value) return;
            killsCount = value;
            ev.killsCount = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IncreaseKillsCount(int relative = 1) => SetKillsCount(killsCount + relative);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReduceKillsCount(int relative = 1) => SetKillsCount(killsCount - relative);
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetGameSpeed(float value)
        {
            if (FloatUtils.IsEquals(gameSpeed, value)) return;
            gameSpeed = value;
            ev.gameSpeed = true;
        }
        #endregion

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void Refresh() {
            // eventsSuspended = false;
            ev.All();
            for (var idx = 0; idx < aspect.LenEx(); idx++)
            {
                aspect.GetExByIndex(idx).Refresh();
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            lives = default;
            maxLives = default;
            //levelNumber = (ushort)(levelNumber > 0 ? levelNumber : 1);
            energy = default;
            maxEnergy = default;
            // nextWaveCountdown = default;
            // waveNumber = default;
            // waveCount = default;
            // activeSpawnCount = default;
            // enemiesCount = default;
            ev.All();
            
            for (var idx = 0; idx < aspect.LenEx(); idx++)
            {
                aspect.GetExByIndex(idx).Clear();
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SendChanges()
        {
            var needSendSome = false;
            
            if (!ev.IsEmpty())
            {
                events.unique.GetOrAdd<Event_StateChanged>() = ev;
                needSendSome = true;
            }
            ev = default;
            
            for (var idx = 0; idx < aspect.LenEx(); idx++)
            {
                var ex = aspect.GetExByIndex(idx);
                if (ex.SendChanges()) needSendSome = true;
            }

            if (needSendSome)
            {
                events.unique.GetOrAdd<Event_StageSomeChanged>();
            }
        }

        
#if UNITY_EDITOR
        public void DrawStateProperties(VisualElement root)
        {
            EditorUtils.Styles.Init();
            
            EditorUtils.DrawProperty("Game Simulation Enabled", simulationEnabled);
            EditorUtils.DrawProperty("Game Speed", gameSpeed);
            //EditorUtils.DrawProperty("Level Number", levelNumber);
            EditorUtils.DrawProperty("Max Lives", maxLives);
            EditorUtils.DrawProperty("Lives", lives);
            EditorUtils.DrawProperty("Max Energy", maxEnergy);
            EditorUtils.DrawProperty("Energy", energy);

            for (var idx = 0; idx < aspect.LenEx(); idx++)
            {
                EditorUtils.DrawLine(1, 10, 0);
                var exName = aspect.GetExByIndex(idx).GetStateName();
                if (EditorUtils.PrimaryFoldoutBegin("state-" + idx, exName))
                {
                    aspect.GetExByIndex(idx).DrawStateProperties(root);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
#endif
    }
}