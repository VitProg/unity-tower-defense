using Leopotam.EcsLite;
using NaughtyAttributes;
using td.common;
using td.components.flags;
using td.features.dragNDrop;
using td.features.shards.events;
using td.features.shards.flags;
using td.utils;
using td.utils.ecs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace td.features.shards.mb
{
    public class ShardUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public ShardMonoBehaviour shardUI;
        public Image plus;
        public TMP_Text costText;

        [OnValueChanged("Refresh")]
        public bool hasShard = false;

        [OnValueChanged("Refresh")]
        public bool showPlus = false;

        [OnValueChanged("Refresh")]
        public int cost = 0;

        public bool druggable = false;

        private void Start()
        {
            shardUI ??= transform.GetComponentInChildren<ShardMonoBehaviour>();
            costText ??= transform.GetComponentInChildren<TMP_Text>();
            plus ??= transform.GetComponentInChildren<Image>();

            Refresh();
        }

        [Button("Refresh UI")]
        public void Refresh()
        {
            shardUI.gameObject.SetActive(hasShard);
            plus.gameObject.SetActive(!hasShard && showPlus);

            if (hasShard)
            {
                shardUI.UpdateFromEntity();
                shardUI.Refresh();
            }

            if (cost > 0 && hasShard)
            {
                costText.text = $"<size=80%>{Constants.UI.CurrencySign}</size>{cost:N0}".Replace(',', '\'');
                costText.gameObject.SetActive(true);
            }
            else
            {
                costText.gameObject.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            shardUI.Hovered = true;
            if (!hasShard || !shardUI.HasShard()) return;
            var world = DI.GetWorld();
            var shardEntity = shardUI.GetShardEntity();
            world.GetComponent<ShardIsHovered>(shardEntity);
            DI.GetShared<SharedData>()?.shardInfo.ShowInfo(ref shardUI.GetShard());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            shardUI.Hovered = false;
            if (!hasShard || !shardUI.HasShard()) return;
            var world = DI.GetWorld();
            var shardEntity = shardUI.GetShardEntity();
            world.DelComponent<ShardIsHovered>(shardEntity);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (druggable && hasShard && Input.GetMouseButtonDown(0) && shardUI.HasShard())
            {
                var world = DI.GetWorld();
                var shardEntity = shardUI.GetShardEntity();
                var alreadyDragged = world.HasComponent<IsDragging>(shardEntity);
                var anyoneAlreadyDragged = world.Filter<Shard>().Inc<IsDragging>().Exc<IsDisabled>()
                    .Exc<IsDestroyed>().End().GetEntitiesCount() > 0;
                
                if (!alreadyDragged && !anyoneAlreadyDragged)
                {
                    var systems = DI.GetSystems();
                    var shared = DI.GetShared<SharedData>()!;
                    
                    ref var downEvent = ref systems.Outer<UIShardDownEvent>();
                    downEvent.packedEntity = world.PackEntity(shardEntity);
                    downEvent.position = CameraUtils.ToWorldPoint(shared.canvasCamera, Input.mousePosition);
                }
            }
        }
    }
}