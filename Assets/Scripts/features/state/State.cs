using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.features.eventBus;
using td.utils;
using UnityEngine.UIElements;

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
        private bool gameSimulationEnabled;
        private float maxLives;
        private float lives;
        private ushort levelNumber;
        private uint energy;
        private float nextWaveCountdown;
        private int waveNumber;
        private int waveCount;
        private int activeSpawnCount;
        private int enemiesCount;
        private float gameSpeed;

        private float lastLives;
        private uint lastEnergy;
        #endregion        

        #region Getters
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public bool GetGameSimulationEnabled() => gameSimulationEnabled;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public float GetMaxLives() => maxLives;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public float GetLastLives() => lastLives;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public float GetLives() => lives;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public ushort GetLevelNumber() => levelNumber;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public uint GetLastEnergy() => lastEnergy;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public uint GetEnergy() => energy;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public float GetNextWaveCountdown() => nextWaveCountdown;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public int GetWaveNumber() => waveNumber;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public int GetWaveCount() => waveCount;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public int GetActiveSpawnCount() => activeSpawnCount;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public int GetEnemiesCount() => enemiesCount;
        [MethodImpl (MethodImplOptions.AggressiveInlining)] public float GetGameSpeed() => gameSpeed;
        #endregion

        #region Setters
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetGameSimulationEnabled(bool value)
        {
            if (gameSimulationEnabled == value) return;
            gameSimulationEnabled = value;
            ev.gameSimulationEnabled = true;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetMaxLives(float value)
        {
            if (FloatUtils.IsEquals(maxLives, value)) return;
            maxLives = value;
            ev.maxLives = true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void SetLastLives(float value) => lastLives = value;
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetLives(float value)
        {
            if (FloatUtils.IsEquals(lives, value)) return;
            SetLastLives(lives);
            lives = value;
            ev.lives = true;
        }
     
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetLevelNumber(ushort value)
        {
            if (levelNumber == value) return;
            levelNumber = value;
            ev.levelNumber = true;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void SetLastEnergy(uint value) => lastEnergy = value;
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetEnergy(uint value)
        {
            if (energy == value) return;
            SetLastEnergy(energy);
            energy = value;
            ev.energy = true;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetNextWaveCountdown(float value)
        {
            if (FloatUtils.IsEquals(nextWaveCountdown, value)) return;
            nextWaveCountdown = value;
            ev.nextWaveCountdown = true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetWaveNumber(int value)
        {
            if (waveNumber == value) return;
            waveNumber = value;
            ev.waveNumber = true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetWaveCount(int value)
        {
            if (waveCount == value) return;
            waveCount = value;
            ev.waveCount = true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetActiveSpawnCount(int value)
        {
            if (activeSpawnCount == value) return;
            activeSpawnCount = value;
            ev.activeSpawnCount = true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetEnemiesCount(int value)
        {
            if (enemiesCount == value) return;
            enemiesCount = value;
            ev.enemiesCount = true;
        }
        
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
            maxLives = default; 
            lives = default;
            levelNumber = (ushort)(levelNumber > 0 ? levelNumber : 1);
            energy = default;
            nextWaveCountdown = default;
            waveNumber = default;
            waveCount = default;
            activeSpawnCount = default;
            enemiesCount = default;
            ev.All();
            
            for (var idx = 0; idx < aspect.LenEx(); idx++)
            {
                aspect.GetExByIndex(idx).Clear();
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SendChanges()
        {
            if (!ev.IsEmpty())
            {
                events.unique.GetOrAdd<Event_StateChanged>() = ev;
            }
            ev = default;
            
            for (var idx = 0; idx < aspect.LenEx(); idx++)
            {
                var ex = aspect.GetExByIndex(idx);
                ex.SendChanges();
            }
        }

        
#if UNITY_EDITOR
        public void DrawStateProperties(VisualElement root)
        {
            EditorUtils.DrawProperty("Game Simulation Enabled", gameSimulationEnabled);
            EditorUtils.DrawProperty("Max Lives", maxLives);
            EditorUtils.DrawProperty("Lives", lives);
            EditorUtils.DrawProperty("Level Number", levelNumber);
            EditorUtils.DrawProperty("Energy", energy);
            EditorUtils.DrawProperty("Next Wave Countdown", nextWaveCountdown);
            EditorUtils.DrawProperty("Wave Number", waveNumber);
            EditorUtils.DrawProperty("Wave Count", waveCount);
            EditorUtils.DrawProperty("Active Spawn Count", activeSpawnCount);
            EditorUtils.DrawProperty("Enemies Count", enemiesCount);
            EditorUtils.DrawProperty("Game Speed", gameSpeed);

            for (var idx = 0; idx < aspect.LenEx(); idx++)
            {
                aspect.GetExByIndex(idx).DrawStateProperties(root);
            }
        }
#endif
    }
}