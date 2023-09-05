using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using td.features.building.buildingShop.state;
using td.features.camera;
using td.features.camera.bus;
using td.features.eventBus;
using td.features.level.bus;
using td.features.state;
using td.utils;
using td.utils.di;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Event_StateChanged = td.features.state.bus.Event_StateChanged;

namespace td.features.building.buildingShop.ui
{
    public class UI_BuildingShop : MonoBehaviour, IPointerClickHandler
    {
        [Required] public RectTransform rectTransform;
        [Required] public GameObject itemsContainer;
        [Required][SerializeField] private GameObject itemPrefab;
        [SerializeField, OnValueChanged("RefreshItemsSize")] private float itemSize = 200f;
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

        private void Start()
        {
            Hide();

            Events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Event_BuildingShop_StateChanged>(OnBuildingShopStateChanged);
            Events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
            Events.unique.ListenTo<Event_Camera_Moved>(OnCameraMoved);
            
            //todo catch camera movement and close buildings menu
        }
        
        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Event_BuildingShop_StateChanged>(OnBuildingShopStateChanged);
            Events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
            Events.unique.RemoveListener<Event_Camera_Moved>(OnCameraMoved);
        }

        //----------------------------------------------------------------
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnStateChanged(ref Event_StateChanged ev)
        {
            if (ev.lives && State.IsDead()) Hide();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnCameraMoved(ref Event_Camera_Moved obj) => UpdatePosition();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnBuildingShopStateChanged(ref Event_BuildingShop_StateChanged ev)
        {
            if (ev is { visible: false, position: false }) return;
            if (ev.items) RefreshItems();
            if (StateEx.GetVisible()) Show();
            else Hide();
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnLevelFinished(ref Event_LevelFinished ev) => Hide();
        
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
            // RefreshItems();
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
                rectTransform.FixAnchoredPosition();
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
            // for (var idx = childCount - 1; idx >= 0; idx--)
            // {
                // Destroy(itemsContainer.transform.GetChild(idx).gameObject);
            // }

            var count = sEx.GetCount();
            ref var buildings = ref sEx.GetBuildings();
            var idx = 0;
            for (; idx < count; idx++)
            {
                ref var building = ref buildings[idx];
                var go = idx < childCount
                    ? itemsContainer.transform.GetChild(idx).gameObject
                    : Instantiate(itemPrefab, itemsContainer.transform);
                var mb = go.GetComponent<UI_BuildingShop_Item>();
                mb.icon = bs.GetIcon(building.id);
                mb.buildingId = building.id;
                mb.title = building.name;
                mb.price = building.price;
                mb.pricetIsGood = currentEnergy >= building.price;
                mb.builtTime = building.buildTime;
                mb.Refresh();
                
                items.Add(mb);
            }

            for (; idx < childCount; idx++)
            {
                Destroy(itemsContainer.transform.GetChild(idx).gameObject);
            }

            RefreshItemsSize();
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void OnPointerClick(PointerEventData eventData)
        {
            StateEx.SetVisible(false);
        }
    }
}