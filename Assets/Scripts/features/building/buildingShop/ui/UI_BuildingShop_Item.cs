using NaughtyAttributes;
using td.features._common;
using td.features.building.buildingShop.bus;
using td.features.building.buildingShop.state;
using td.features.eventBus;
using td.features.state;
using td.utils.di;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace td.features.building.buildingShop.ui
{
    public class UI_BuildingShop_Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [BoxGroup("links"), Required, OnValueChanged("Refresh")]
        public TMP_Text tTitle;
        
        [BoxGroup("links"), Required, OnValueChanged("Refresh")]
        public TMP_Text tTime;
        
        [BoxGroup("links"), Required, OnValueChanged("Refresh")]
        public TMP_Text tPriseBad;
        
        [BoxGroup("links"), Required, OnValueChanged("Refresh")]
        public TMP_Text tPriceGood;
        
        [BoxGroup("links"), Required, OnValueChanged("Refresh")]
        public Image objImage;
        
        [BoxGroup("links"), Required, OnValueChanged("Refresh")]
        public Image bg;
        
        [BoxGroup("Colors"), OnValueChanged("Refresh")]
        public Color normalColor = Color.white;
        
        [BoxGroup("Colors"), OnValueChanged("Refresh")]
        public Color hoverColor = Color.cyan;
        
        [BoxGroup("Icon"), OnValueChanged("Refresh")]
        public Color iconColor = Color.white;
        [Required, BoxGroup("Icon"), OnValueChanged("Refresh")]
        public Sprite icon;

        [FormerlySerializedAs("id")] [OnValueChanged("Refresh")] public string buildingId;
        [OnValueChanged("Refresh")] public string title;
        [OnValueChanged("Refresh")] public string description;
        [OnValueChanged("Refresh")] public uint builtTime;
        [OnValueChanged("Refresh")] public uint price;
        [FormerlySerializedAs("prisetIsGood")] [OnValueChanged("Refresh")] public bool pricetIsGood = true;
        
        //
        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();
        private State _state;
        private State State =>  _state ??= ServiceContainer.Get<State>();
        //

        public void OnPointerEnter(PointerEventData eventData)
        {
            bg.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            bg.color = normalColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var s = State.Ex<BuildingShop_StateEx>();
            ref var cmd = ref Events.global.Add<Command_BuyBuilding>();
            cmd.buildingId = buildingId;
            cmd.cellCoords = s.GetCellCoords();
            cmd.price = price;
            cmd.buildTime = builtTime;
            s.SetVisible(false);
        }

        [Button]
        public void Refresh()
        {
            if (!bg) return;
            bg.color = normalColor;

            tTitle.text = title;
            tPriceGood.text = CommonUtils.PriceFormat(price);
            tPriseBad.text = CommonUtils.PriceFormat(price);
            
            tPriceGood.gameObject.SetActive(pricetIsGood);
            tPriseBad.gameObject.SetActive(!pricetIsGood);

            objImage.sprite = icon;
            objImage.color = iconColor;
            
            tTime.text = CommonUtils.TMPTimeFormat(builtTime);
        }
    }
}