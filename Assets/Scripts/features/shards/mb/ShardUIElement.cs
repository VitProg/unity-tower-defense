using Leopotam.EcsLite;
using td.components.flags;
using td.features.dragNDrop;
using td.features.shards.events;
using td.features.shards.flags;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shards.mb
{
#if UNITY_EDITOR
    [SelectionBase]
#endif
    public class ShardUIElement : MonoBehaviour
    {
        private EcsEntity ecsEntity;
        private RectTransform rectTransform;
        private RectTransform parentRectTransform;
        private Canvas canvas;
        private Image hover;
        private ShardMonoBehaviour shardMB;
        private GridLayoutGroup grid;

        private EcsWorld world;
        private IEcsSystems systems;
        private ShardInfoPanel infoPanel;
        private ShardUIButton shardUIButton;

        private Vector2 Size
        {
            get
            {
                if (grid)
                {
                    var gridScaleFactor = grid.cellSize / parentRectTransform.rect.size;
                    return rectTransform.rect.size * gridScaleFactor * canvas.scaleFactor;
                }
                return rectTransform.rect.size * canvas.scaleFactor;
            }
        }

        private float Radius
        {
            get
            {
                var size = Size;
                return Mathf.Min(size.x, size.y) / 2f;
            }
        }

        protected void Start()
        {
            canvas = GetComponentInParent<Canvas>().rootCanvas;
            ecsEntity ??= GetComponentInParent<EcsEntity>();
            shardMB ??= GetComponentInParent<ShardMonoBehaviour>();
            shardUIButton ??= GetComponentInParent<ShardUIButton>();
            parentRectTransform ??= GetComponentInParent<RectTransform>();
            rectTransform ??= GetComponent<RectTransform>();
            hover ??= transform.parent.Find("hover").GetComponent<Image>();
            grid ??= GetComponentInParent<GridLayoutGroup>();
            infoPanel ??= FindObjectOfType<ShardInfoPanel>();
        }

        // protected void Update()
        // {
        //     // Todo optimize
        //     var radius = Radius;
        //     var sqrRadius = radius * radius;
        //     var distance = ((Vector2)Input.mousePosition - (Vector2)rectTransform.position).sqrMagnitude;
        //     
        //     if (!ecsEntity || !ecsEntity.TryGetEntity(out var shardEntity)) return;
        //         
        //     systems ??= DI.GetSystems();
        //     world ??= DI.GetWorld();
        //
        //     if (distance < sqrRadius)
        //     {
        //         if (shardMB) hover.color = ShardUtils.GetHoverColor(shardMB.Values, hover.color.a, shardMB.config);
        //         hover.gameObject.SetActive(true);
        //         world.GetComponent<ShardIsHovered>(shardEntity);
        //
        //         var canDrag = shardUIButton?.druggable ?? true;
        //
        //         if (canDrag && Input.GetMouseButtonDown(0))
        //         {
        //             var alreadyDragged = world.HasComponent<IsDragging>(shardEntity);
        //             var anyoneAlreadyDragged = world.Filter<Shard>().Inc<IsDragging>().Exc<IsDisabled>()
        //                 .Exc<IsDestroyed>().End().GetEntitiesCount() > 0;
        //             
        //             if (!alreadyDragged && !anyoneAlreadyDragged)
        //             {
        //                 ref var downEvent = ref systems.Outer<UIShardDownEvent>();
        //                 downEvent.packedEntity = world.PackEntity(shardEntity);
        //                 downEvent.position = Input.mousePosition;
        //             }
        //         }
        //
        //         if (infoPanel && shardMB.HasShard())
        //         {
        //             ref var shard = ref shardMB.GetShard();
        //             infoPanel.ShowInfo(ref shard);
        //         }
        //     }
        //     else
        //     {
        //         world.DelComponent<ShardIsHovered>(shardEntity);
        //         hover.gameObject.SetActive(false);
        //     }
        // }
    }
}