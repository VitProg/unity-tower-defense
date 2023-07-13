using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using td.features.shards.config;
using td.monoBehaviours;
using td.utils.ecs;
using TMPro;
using Unity.Collections;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace td.features.shards.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    // [ExecuteAlways]
    public class ShardMonoBehaviour : MonoBehaviour // required interface when using the OnPointerEnter method.
    {
        [Required][OnValueChanged("Refresh")] public ShardsConfig config;

        [OnValueChanged("Refresh")] [MaxValue(100)] public byte red;
        [OnValueChanged("Refresh")] [MaxValue(100)] public byte green;
        [OnValueChanged("Refresh")] [MaxValue(100)] public byte blue;
        [OnValueChanged("Refresh")] [MaxValue(100)] public byte aquamarine;
        [OnValueChanged("Refresh")] [MaxValue(100)] public byte yellow;
        [OnValueChanged("Refresh")] [MaxValue(100)] public byte orange;
        [OnValueChanged("Refresh")] [MaxValue(100)] public byte pink;
        [OnValueChanged("Refresh")] [MaxValue(100)] public byte violet;

        [OnValueChanged("Refresh")] public Image hover;
        [Required][OnValueChanged("Refresh")] public GameObject levelIndicator;

        [ShowNativeProperty] public uint Quantity => (uint)red + green + blue + aquamarine + yellow + orange + pink + violet;

        [ShowNativeProperty] public int Level => config.GetLevelCoefficient((int)Quantity);

        public byte[] Values => new[] { red, green, blue, aquamarine, yellow, orange, pink, violet };

        private uint lastQuantity = 0;

        [SerializeField][Required] public EcsEntity ecsEntity;
        [SerializeField][Required] private ShardMeshGenerator shardMeshGenerator;
        private ShardCalculator shardCalculator;
        private SpriteRenderer levelIndicatorSpriteRenderer;
        private Image levelIndicatorImage;

        // private readonly List<ColorItem> colors = new ();
        // private int prevColor;
        // private int currentColor;
        // private int nextColor;
        // private float colorTime;

        private void Start()
        {
            ecsEntity ??= GetComponent<EcsEntity>();
        }

        private void Update()
        {
            var quantity = Quantity;
            if (lastQuantity != quantity)
            {
                Refresh();
                lastQuantity = quantity;
            }

            // if (levelIndicator)
            // {
                // UpdateColor();
            // }
        }

       
        [Button("Update Mesh", EButtonEnableMode.Editor)]
        private void UpdateShader()
        {
            Refresh();
        }
        
        public void Refresh()
        {
            if (shardCalculator == null)
            {
                shardCalculator = DI.GetCustom<ShardCalculator>();
            }
            
            shardMeshGenerator.colorWeights = ShardUtils.ToArray(red, green, blue, aquamarine, yellow, orange, pink, violet);

            if (HasShard())
            {
                shardMeshGenerator.innerRadius = Mathf.Lerp(
                    0.6f,
                    0.15f,
                    Level / 15f
                );
            }

            shardMeshGenerator.Refresh();

            if (hover != null)
            {
                var hoverColor = ShardUtils.GetHoverColor(Values, hover.color.a, config);
                hover.color = hoverColor;
            }

            if (levelIndicator != null)
            {
                var mixedColor = ShardUtils.GetMixedColor(Values, config);
                levelIndicatorImage ??= levelIndicator.GetComponent<Image>();
                if (levelIndicatorImage != null)
                {
                    levelIndicatorImage.color = mixedColor;
                    if (config.levelSprites.Length > Level - 1)
                    {
                        levelIndicatorImage.sprite = config.levelSprites[Level - 1];
                    }
                }
                else
                {
                    levelIndicatorSpriteRenderer ??= levelIndicator.GetComponent<SpriteRenderer>();
                    levelIndicatorSpriteRenderer.color = mixedColor;
                    if (config.levelSprites.Length > Level - 1)
                    {
                        levelIndicatorSpriteRenderer.sprite = config.levelSprites[Level - 1];
                    }
                }
            }

            // ShardUtils.GetColors(red, green, blue, aquamarine, yellow, orange, pink, violet, config, colorsForLevelIndicator);
            //ToDo Move to ShardInitColorSystem
            // colors.Clear();
            // var q = Quantity;
            // if (red > 0) colors.Add(new ColorItem { color = config.redShardColor, weight = red / (float)q });
            // if (green > 0) colors.Add(new ColorItem { color = config.greenShardColor, weight = green / (float)q });
            // if (blue > 0) colors.Add(new ColorItem { color = config.blueShardColor, weight = blue / (float)q });
            // if (aquamarine > 0) colors.Add(new ColorItem { color = config.aquamarineShardColor, weight = aquamarine / (float)q });
            // if (yellow > 0) colors.Add(new ColorItem { color = config.yellowShardColor, weight = yellow / (float)q });
            // if (orange > 0) colors.Add(new ColorItem { color = config.orangeShardColor, weight = orange / (float)q });
            // if (pink > 0) colors.Add(new ColorItem { color = config.pinkShardColor, weight = pink / (float)q });
            // if (violet > 0) colors.Add(new ColorItem { color = config.violetShardColor, weight = violet / (float)q });
            // currentColor = 0;
            // nextColor = colors.Count > 1 ? 1 : 0;
            // prevColor = colors.Count - 1;
            // if (colors.Count > 0) SetLevelAndHoverColor(colors[currentColor].color);
        }

        public void SetLevelAndHoverColor(Color c)
        {
            if (hover) hover.color = new Color(c.r, c.g, c.b, hover.color.a);
            if (levelIndicatorImage) levelIndicatorImage.color = c;
            if (levelIndicatorSpriteRenderer) levelIndicatorSpriteRenderer.color = c;
        }
        
        //ToDo Move to ShardAnimateColorSystem
//         private void UpdateColor()
//         {
//             /*
//              * *    {"blue": 1},
//     {"violet": 1},
//     {"aquamarine":  1, "red":  2},
//     {"aquamarine":  10},
//     {"aquamarine":  20}
//
//              */
//             if (colors.Count <= 1) return;
//
//             colorTime += Time.deltaTime / 10f;
//             
//             // const float speed = 50f;
//             
//             var prev = colors[prevColor];
//             var current = colors[currentColor];
//             var next = colors[nextColor];
//
//             var color = current.color;
//
//             var w = colors[currentColor].weight * 5f;
//             const float fade = .1f;
//
//             if (colorTime < fade)
//             {
//                 var t = colorTime / fade;
//                 color = Color.Lerp(prev.color, current.color, t);
//                 // Debug.Log($"Lerp color: prev -> current | {colorTime} | {t}");
//             }
//             else if (colorTime > w + fade)
//             {
//                 var t = ((current.weight + fade + fade) - colorTime) / fade;
//                 color = Color.Lerp(current.color, next.color, t);
//                 // Debug.Log($"Lerp color: current -> next | {colorTime} | {t}");
//             }
//             // else
//             // {
//                 // Debug.Log($"Color | {colorTime}");
//             // }
//
//             SetLevelAndHoverColor(color);
//             
//             if (colorTime > colors[currentColor].weight + fade + fade)
//             {
//                 colorTime = 0f;
//                 currentColor = (currentColor + 1) % colors.Count;
//                 nextColor = (currentColor + 1) % colors.Count;
//                 prevColor = (currentColor - 1 + colors.Count) % colors.Count;
//                 // Debug.Log("prevColor = " + prevColor);
//                 // Debug.Log("currentColor = " + currentColor);
//                 // Debug.Log("nextColor = " + nextColor);
//                 // Debug.Log("----------------------------------------");
//             }
//
//             // // if (colorsForLevelIndicator.em == 0) return;
//             //
//             // // var colorIndex = Mathf.CeilToInt(indexColorsForLevelIndicator) % colorsForLevelIndicator.Length;
//             // levelIndicatorCurrentColor = colorsForLevelIndicator[indexColorsForLevelIndicator];
//             //
//             // var c = levelIndicatorCurrentColor;
//             //
//             // if (levelIndicatorLastColor == null)
//             // {
//             //     levelIndicatorLastColor = levelIndicatorCurrentColor;
//             // }
//             //
//             // levelIndicatorColorChangeTime += Time.deltaTime;
//             // c = Color.Lerp(
//             //     levelIndicatorLastColor.Value,
//             //     levelIndicatorCurrentColor,
//             //     levelIndicatorColorChangeTime
//             // );
//             //
//             // if (levelIndicatorColorChangeTime > 1f)
//             // {
//             //     levelIndicatorLastColor = levelIndicatorCurrentColor;
//             //     indexColorsForLevelIndicator = (indexColorsForLevelIndicator + 1) % colorsForLevelIndicator.Count;
//             //     levelIndicatorColorChangeTime = 0f;
//             // }
//             //
//             // if (levelIndicatorImage) levelIndicatorImage.c = c;
//             // if (levelIndicatorSpriteRenderer) levelIndicatorSpriteRenderer.c = c;
//             //
//             // // indexColorsForLevelIndicator += Time.deltaTime;
//         }

        
        void OnEnable()
        {
            Refresh();
            // StartCoroutine(IdleRefresh());
        }

        private IEnumerator IdleRefresh()
        {
            Refresh();
            yield return new WaitForSeconds(1.25f);
            Refresh();
        }

        public bool HasShard() => ecsEntity != null && ecsEntity.TryGetEntity(out var entity) &&
                   DI.GetWorld().HasComponent<Shard>(entity);
        
        public ref Shard GetShard()
        {
            if (HasShard() && ecsEntity.TryGetEntity(out var entity))
            {
                var world = DI.GetWorld();
                ref var shard = ref world.GetComponent<Shard>(entity);
                return ref shard;
            }

            throw new NullReferenceException("Shard is null");
        }       
        
        public int GetShardEntity()
        {
            if (HasShard() && ecsEntity.TryGetEntity(out var shardEntity))
            {
                return shardEntity;
            }

            throw new NullReferenceException("Shard is null");
        }

        [Button("Update fields from Entity", EButtonEnableMode.Playmode)]
        public void UpdateFromEntity()
        {
            if (HasShard())
            {
                ref var shard = ref GetShard();

                red = shard.red;
                green = shard.green;
                blue = shard.blue;
                aquamarine = shard.aquamarine;
                yellow = shard.yellow;
                orange = shard.orange;
                pink = shard.pink;
                violet = shard.violet;

                Refresh();
            }
        }

        public bool Hovered
        {
            get => hover.gameObject.activeSelf;
            set
            {
                hover.gameObject.SetActive(value);
            }
        }
    }

    internal struct ColorItem
    {
        public Color color;
        public float weight;
    }
}