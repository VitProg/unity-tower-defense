using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.shard.bus;
using td.features.shard.components;
using td.features.shard.shardCollection;
using td.features.state;
using UnityEngine;

namespace td.features.shard.systems
{
    public class Shard_DropHandler_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private State state;

        private ShardCollection_State _collectionState;
        private ShardCollection_State CollectionState => _collectionState ??= state.Ex<ShardCollection_State>();

        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Command_DropShard_OnMap>(OnDrop);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Command_DropShard_OnMap>(OnDrop);
        }

        // ----------------------------------------------------------------

        private void OnDrop(ref Command_DropShard_OnMap cmd)
        {
            if (cmd.cost <= 0 || !state.IsEnoughEnergy(cmd.cost)) return;
            if (!CollectionState.HasItem(cmd.sourceIndex)) return;
            
            state.ReduceEnergy(cmd.cost);
            
            //todo
            Debug.Log("SHARD DROPPED ON MAP!!!");

            ref var ev = ref events.global.Add<Event_ShardDropped_OnMap>();
            ev.shard = CollectionState.GetItem(cmd.sourceIndex);
            ev.position = cmd.position;
            
            //todo

            CollectionState.RemoveItemAt(cmd.sourceIndex);
        }
    }
}