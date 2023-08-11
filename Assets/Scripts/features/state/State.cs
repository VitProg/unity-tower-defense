using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.costPopup;
using td.features.eventBus;
using td.features.infoPanel;
using td.features.shard.shardCollection;
using td.features.shard.shardStore;
using td.utils;
using UnityEngine.UIElements;

namespace td.features.state
{
    // public interface IState
    // {
    //     float MaxLives { get; set; }
    //     float Lives { get; set; }
    //     ushort LevelNumber { get; set; }
    //     uint Energy { get; set; }
    //     float NextWaveCountdown { get; set; }
    //     int WaveNumber { get; set; }
    //     int WaveCount { get; set; }
    //     int ActiveSpawnCount { get; set; }
    //     int EnemiesCount { get; set; }
    //     float GameSpeed { get; set; }
    //     float LastLives { get; }
    //     uint LastEnergy { get; }
    //
    //     InfoPanel_State InfoPanel { get; }
    //     CostPopup_State CostPopup { get; }
    //     ShardStore_State ShardStore { get; }
    //     ShardCollection_State ShardCollection { get; }
    //     
    //     ref Event_StateChanged GetEvent();
    //     void SuspendEvents();
    //     void ResumeEvents();
    //     void Refresh();
    //     void UnSuspend();
    //     void Clear();
    // }

    public interface IStateExtension
    {
#if UNITY_EDITOR
        void DrawStateProperties();
#endif
    }
    
    public class State
    {
        // private readonly EcsInject<IEventBus> events = default;
        [DI] private readonly EventBus events;
        
        private Slice<IStateExtension> extensions = new();
        private Dictionary<Type, int> extensionsHash = new (10);

        public T Ex<T>() where T : IStateExtension
        {
           var type = typeof(T);
           if (!extensionsHash.TryGetValue(type, out var idx))
           {
               throw new Exception($"State extension {type.Name} not registered");
           }

           return (T)extensions.Get(idx);
        }

        public void AddEx<T>(T ex) where T : IStateExtension
        {
            var type = typeof(T);
            if (extensionsHash.TryGetValue(type, out _))
            {
                throw new Exception($"State extension {type.Name} already registered");
            }

            extensions.Add(ex);
            var idx = extensions.Len() - 1;
            extensionsHash[type] = idx;
        }
        
#if UNITY_EDITOR
        void DrawStateProperties(VisualElement root)
        {
            //todo 
        }
#endif
        
        // -----
        
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
        private InfoPanel_State infoPanel;
        private CostPopup_State costPopup;
        private ShardStore_State shardStore;
        private ShardCollection_State shardCollection;

        public InfoPanel_State InfoPanel => infoPanel;
        public CostPopup_State CostPopup => costPopup;
        public ShardStore_State ShardStore => shardStore;
        public ShardCollection_State ShardCollection => shardCollection;

        // -----
        
        private bool eventsSuspended;
        private Event_StateChanged suspendEvent = new ();
        private bool hasSuspendEvent = false;

        public State()
        {
            infoPanel = new InfoPanel_State(this);
            costPopup = new CostPopup_State(this);
            shardStore = new ShardStore_State(this);
            shardCollection = new ShardCollection_State(this);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref Event_StateChanged GetEvent()
        {
            if (!eventsSuspended) return ref events.unique.GetOrAdd<Event_StateChanged>();
            return ref suspendEvent;
        }
        
        public float MaxLives
        {
            get => maxLives;
            set
            {
                if (FloatUtils.IsEquals(maxLives, value)) return;
                maxLives = value;
                GetEvent().maxLives = true;
            }
        }

        public float LastLives { get; private set; }
        public float Lives
        {
            get => lives;
            set
            {
                if (FloatUtils.IsEquals(lives, value)) return;
                LastLives = lives;
                lives = value;
                GetEvent().lives = true;
            }
        }
        
        public ushort LevelNumber
        {
            get => levelNumber;
            set
            {
                if (levelNumber == value) return;
                levelNumber = value;
                GetEvent().levelNumber = true;
            }
        }

        public uint LastEnergy { get; private set; }
        public uint Energy
        {
            get => energy;
            set
            {
                if (energy == value) return;
                LastEnergy = energy;
                energy = value;
                GetEvent().energy = true;
            }
        }

        public float NextWaveCountdown
        {
            get => nextWaveCountdown;
            set
            {
                if (FloatUtils.IsEquals(nextWaveCountdown, value)) return;
                nextWaveCountdown = value;
                GetEvent().nextWaveCountdown = true;
            }
        }

        public int WaveNumber
        {
            get => waveNumber;
            set
            {
                if (waveNumber == value) return;
                waveNumber = value;
                GetEvent().waveNumber = true;
            }
        }

        public int WaveCount
        {
            get => waveCount;
            set
            {
                if (waveCount == value) return;
                waveCount = value;
                GetEvent().waveCount = true;
            }
        }

        public int ActiveSpawnCount
        {
            get => activeSpawnCount;
            set
            {
                if (activeSpawnCount == value) return;
                activeSpawnCount = value;
                GetEvent().activeSpawnCount = true;
            }
        }

        public int EnemiesCount
        {
            get => enemiesCount;
            set
            {
                if (enemiesCount == value) return;
                enemiesCount = value;
                GetEvent().enemiesCount = true;
            }
        }
        
        public float GameSpeed
        {
            get => gameSpeed;
            set
            {
                if (FloatUtils.IsEquals(gameSpeed,value)) return;
                gameSpeed = value;
                GetEvent().gameSpeed = true;
            }
        }
        
        
        

        public void SuspendEvents() => eventsSuspended = true;
        public void ResumeEvents()
        {
            eventsSuspended = false;
            if (hasSuspendEvent)
            {
                events.unique.GetOrAdd<Event_StateChanged>() = suspendEvent;
                UnSuspend();
            }
        }

        public void Refresh() {
            eventsSuspended = false;
            UnSuspend();
            events.unique.GetOrAdd<Event_StateChanged>().All();
        }

        public void UnSuspend()
        {
            hasSuspendEvent = false;
            suspendEvent.Clear();
        }

        public void Clear()
        {
            maxLives = default; 
            lives = default;
            levelNumber = default;
            energy = default;
            nextWaveCountdown = default;
            waveNumber = default;
            waveCount = default;
            activeSpawnCount = default;
            enemiesCount = default;
            // gameSpeed = 1f
            costPopup.Clear();
            shardStore.Clear();
            shardCollection.Clear();           
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}