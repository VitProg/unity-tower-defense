using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.building;
using td.features.eventBus;
using td.features.level;
using td.features.level.cells;
using td.features.shard;
using td.features.shard.components;
using td.features.shard.shardCollection;
using td.features.shard.shardStore;
using td.features.state;
using td.features.tower;
using td.features.tower.bus;
using td.utils;
using UnityEngine;

namespace td.features.infoPanel.systems {
    public class InfoPanel_System : IProtoInitSystem, IProtoDestroySystem {
        [DI] private EventBus events;
        [DI] private State state;
        [DI] private Level_State levelState;
        [DI] private Building_Service buildingService;
        [DI] private Tower_Service towerService;
        [DI] private Shard_Service shardService;

        private InfoPanel_State _infoPanelState;
        private InfoPanel_State InfoPanelState => _infoPanelState ??= state.Ex<InfoPanel_State>();

        private ShardStore_State _shardStoreState;
        private ShardStore_State ShardStoreState => _shardStoreState ??= state.Ex<ShardStore_State>();

        private ShardCollection_State _shardCollectionState;
        private ShardCollection_State ShardCollectionState => _shardCollectionState ??= state.Ex<ShardCollection_State>();

        public void Init(IProtoSystems systems) {
            events.global.ListenTo<Event_Tower_Hovered>(OnTowerHovered);
            events.global.ListenTo<Event_Tower_UnHovered>(OnTowerHoveredUnHovered);
            events.unique.ListenTo<Event_ShardStore_StateChanged>(OnShardStoreStateChanged);
            events.unique.ListenTo<Event_ShardCollection_StateChanged>(OnShardCollectionStateChanged);
        }

        public void Destroy() {
            events.global.RemoveListener<Event_Tower_Hovered>(OnTowerHovered);
            events.global.RemoveListener<Event_Tower_UnHovered>(OnTowerHoveredUnHovered);
            events.unique.RemoveListener<Event_ShardStore_StateChanged>(OnShardStoreStateChanged);
            events.unique.RemoveListener<Event_ShardCollection_StateChanged>(OnShardCollectionStateChanged);
        }

        // ----------------------------------------------------------------

        private void OnTowerHovered(ref Event_Tower_Hovered ev) {
            ref var building = ref buildingService.GetBuilding(ev.Tower, out _);
            if (building.id != Constants.Buildings.ShardTower) return;
            if (!levelState.HasCell(building.coords, CellTypes.CanBuild)) return;
            ref var cell = ref levelState.GetCell(building.coords, CellTypes.CanBuild);
            if (!cell.HasShard()) return;

            InfoPanelState.Clear();
            InfoPanelState.SetShard(ref shardService.GetShard(cell.packedShardEntity, out _));
            InfoPanelState.SetTitle("Tower");
            InfoPanelState.SetVisible(true);
        }

        private void OnTowerHoveredUnHovered(ref Event_Tower_UnHovered ev) {
            ref var building = ref buildingService.GetBuilding(ev.Tower, out _);
            if (building.id != Constants.Buildings.ShardTower) return;
            if (!levelState.HasCell(building.coords, CellTypes.CanBuild)) return;
            ref var cell = ref levelState.GetCell(building.coords, CellTypes.CanBuild);
            if (!cell.HasShard()) return;

            ref var shard = ref shardService.GetShard(cell.packedShardEntity, out _);

            if (InfoPanelState.HasShard() && InfoPanelState.GetShard()._id_ == shard._id_) InfoPanelState.Clear();
        }

        private void OnShardCollectionStateChanged(ref Event_ShardCollection_StateChanged ev) {
            if (ev is { operation: false, hoveredIndex: false }) return;
            
            var s = ShardCollectionState;

            if ((ev.operation && s.IsAnyOperation()) || s.HasHovered()) {
                InfoPanelState.Clear();
                
                if (s.IsCombineOperation()) InfoPanelState.SetTitle("Combine shards");
                if (s.IsInsertOperation()) InfoPanelState.SetTitle("Insert shard");
                if (s.IsDropOperation()) InfoPanelState.SetTitle("Explode shard");
                
                InfoPanelState.SetPrice(s.GetOperationPrice());
                if (s.IsCombineOperation() || s.IsCombineOperation()) InfoPanelState.SetTime(s.GetOperationTime());
                
                InfoPanelState.SetVisible(true);

                if (s.IsCombineOperation()) {
                    InfoPanelState.SetShard(ref s.GetCombinedShard());                    
                } else if (s.IsInsertOperation()) {
                    InfoPanelState.SetShard(ref s.GetDraggableShard());                    
                } else if (s.HasHovered()) {
                    InfoPanelState.SetShard(ref s.GetItem(s.GetHoveredIndex()));
                }

                if (!InfoPanelState.HasShard()) {
                    InfoPanelState.Clear();
                }
            } else {
                InfoPanelState.Clear();
            }
        }

        private void OnShardStoreStateChanged(ref Event_ShardStore_StateChanged ev) {
            if (!ev.hoveredIndex) return;

            if (ShardStoreState.HasHovered()) {
                ref var shardItem = ref ShardStoreState.GetItem(ShardStoreState.GetHoveredIndex());

                InfoPanelState.Clear();
                InfoPanelState.SetShard(ref shardItem.shard);
                InfoPanelState.SetTitle("Buying shard");
                InfoPanelState.SetPrice(shardItem.shard.price);
                InfoPanelState.SetVisible(true);
                
                InfoPanelState.SetShard(ref shardItem.shard);
            } else {
                InfoPanelState.Clear();
            }
        }
    }
}
