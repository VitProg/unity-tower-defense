using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using td.features.building.buildingShop.state;
using td.features.camera;
using td.features.camera.bus;
using td.features.eventBus;
using td.features.gameStatus.bus;
using td.features.level.bus;
using td.features.state;
using td.utils;
using td.utils.di;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.building.buildingShop.ui
{
    public class UI_BuildingShop : MonoBehaviour
    {
        [Required] public RectTransform rectTransform;
        [Required] public GameObject itemsContainer;
        [Required][SerializeField] private GameObject itemPrefab;
        [SerializeField, OnValueChanged("RefreshSize")] private float itemSize = 200f;
        public List<UI_BuildingShop_Item> items;
        
        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();
        
        private State _state;
        private State State => _state ??= ServiceContainer.Get<State>();

        private BuildingShop_StateEx _stateEx;
        private BuildingShop_StateEx StateEx => _stateEx ??= State.Ex<BuildingShop_StateEx>();

        private Building_Service _buildingService;
        private Building_Service BuildingService => _buildingService ??= ServiceContainer.Get<Building_Service>();

        private Camera_Service _cameraService;
        private Camera_Service CameraService => _cameraService ??= ServiceContainer.Get<Camera_Service>();
        
        private int2 cellCoords;

        [Button]
        public void Refresh()
        {
            items.Clear();
            var count = itemsContainer.transform.childCount;
            for (var idx = 0; idx < count; idx++)
            {
                var i = itemsContainer.transform.GetChild(idx).GetComponent<UI_BuildingShop_Item>();
                if (i) items.Add(i);
            }
        }

        private void Start()
        {
            Hide();
            
            Events.unique.ListenTo<Event_BuildingShop_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
            Events.unique.ListenTo<Event_YouDied>(OnYouDied);
            Events.unique.ListenTo<Event_Camera_Moved>(OnCameraMoved);
            
            //todo catch camera movement and close buildings menu
        }

        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_BuildingShop_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
            Events.unique.RemoveListener<Event_YouDied>(OnYouDied);
            Events.unique.RemoveListener<Event_Camera_Moved>(OnCameraMoved);
        }

        //----------------------------------------------------------------
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnCameraMoved(ref Event_Camera_Moved obj)
        {
            UpdatePosition();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnStateChanged(ref Event_BuildingShop_StateChanged ev)
        {
            if (ev is { visible: false, position: false }) return;
            if (StateEx.GetVisible()) Show();
            else Hide();
        }

        private void OnLevelFinished(ref Event_LevelFinished ev)
        {
            Hide();
        }

        private void OnYouDied(ref Event_YouDied ev)
        {
            Hide();
        }
        
        //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Hide()
        {
            gameObject.SetActive(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Show()
        {
            cellCoords = StateEx.GetCellCoords();
            RefreshItems();
            gameObject.SetActive(true);
            UpdatePosition();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePosition()
        {
            if (gameObject.activeSelf)
            {
                var worldPoint = HexGridUtils.CellToPosition(cellCoords);
                var canvasPoint = CameraService.MainToCanvas(worldPoint);
                transform.position = canvasPoint;
                rectTransform.FixAnchoeredPosition();
            }
        }

        [Button]
        private void RefreshItemsSize()
        {
            foreach (var item in items)
            {
                var rt = (RectTransform)item.gameObject.transform;
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSize);
            }
        }

        private void RefreshItems() {
            var s = State;
            var sEx = StateEx;
            var bs = BuildingService;
            var currentEnergy = s.GetEnergy();
            
            items.Clear();
            var childCount = itemsContainer.transform.childCount;
            for (var idx = childCount - 1; idx >= 0; idx--)
            {
                Destroy(itemsContainer.transform.GetChild(idx).gameObject);
            }

            var count = sEx.GetCount();
            ref var buildings = ref sEx.GetBuildings();
            for (var idx = 0; idx < count; idx++)
            {
                ref var building = ref buildings[idx];
                var go = Instantiate(itemPrefab, itemsContainer.transform);
                var mb = go.GetComponent<UI_BuildingShop_Item>();
                mb.icon = bs.GetIcon(building.id);
                mb.title = building.name;
                mb.price = building.price;
                mb.pricetIsGood = currentEnergy >= building.price;
                mb.builtTime = building.buildTime;
                
                items.Add(mb);
            }

            RefreshItemsSize();
        }
    }
}