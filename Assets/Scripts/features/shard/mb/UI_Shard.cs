using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using NaughtyAttributes;
using td.features.shard.components;
using td.features.shard.data;
using td.utils;
using td.utils.di;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shard.mb {
    public class UI_Shard : MonoBehaviour {
        // Warning - this shard downt link with shard in ECS !!!
        [OnValueChanged("Refresh")] public Shard shard;

        [Required] [SerializeField] private GameObject body;
        [Required] [SerializeField] private UI_Shard_Level level;
        [Required] [SerializeField] private UI_Shard_Hover hover;
        [Required] [SerializeField] [ShowIf("IsCanvasMode")] private UI_Shard_Deny deny;
        [Required] [SerializeField] private Shards_Config_SO shardConfig;

        [SerializeField] [ReadOnly] private Sector[] sectors = new Sector[8];
        [SerializeField] [ReadOnly] private ShardTypes[] colors = new ShardTypes[Constants.UI.Shard.MaxCachedColors];
        [SerializeField] [ReadOnly] private int sectorsCount = 0;
        [SerializeField] [ReadOnly] internal float colorTime = 0f;
        [SerializeField] [ReadOnly] internal float rotation = 0f;

        public EcsEntity ecsEntity;
        public int collectionIndex = -1;
        [SerializeField] private SpriteRenderer[] layersSR;
        [SerializeField] private Image[] layersIM;

        [ShowNativeProperty] private bool IsSpriteRendererMode => layersSR.Length > 0;
        [ShowNativeProperty] private bool IsCanvasMode => layersIM.Length > 0;

#if UNITY_EDITOR && false
#region DEBUG
        [ShowNativeProperty] private float sector0 => sectorsCount > 0 ? sectors[0].weight : 0f;
        [ShowNativeProperty] private float sector1 => sectorsCount > 1 ? sectors[1].weight : 0f; 
        [ShowNativeProperty] private float sector2 => sectorsCount > 2 ? sectors[2].weight : 0f;
        [ShowNativeProperty] private float sector3 => sectorsCount > 3 ? sectors[3].weight : 0f;
        [ShowNativeProperty] private float sector4 => sectorsCount > 4 ? sectors[4].weight : 0f;
        [ShowNativeProperty] private float sector5 => sectorsCount > 5 ? sectors[5].weight : 0f;
        [ShowNativeProperty] private float sector6 => sectorsCount > 6 ? sectors[6].weight : 0f;
        [ShowNativeProperty] private float sector7 => sectorsCount > 7 ? sectors[7].weight : 0f;
#endregion
#endif

        private void Awake() {
            ecsEntity ??= GetComponent<EcsEntity>();
            var mbService = ServiceContainer.Get<Shard_MB_Service>();
            mbService?.Add(this);
            FullRefresh();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetAll() {
            SetHover(false);
            colorTime = RandomUtils.Range(0f, 10f);
            rotation = RandomUtils.Range(0f, 180f);
            ShardUtils.Clear(ref shard);
            level.SetColor(Constants.TransparentColor);
            FullRefresh();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetHover(bool b) {
            if (b) hover.Show();
            else hover.Hide();
        }

        public bool IsHovered {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => hover.gameObject.activeSelf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDeny(bool b) {
            if (b) deny.Show();
            else deny.Hide();
        }

        public bool IsDeny {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => deny.gameObject.activeSelf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetupFromAnother(in UI_Shard uiShard, bool copyShardID = false) {
            shard = uiShard.shard;
            if (copyShardID) shard._id_ = uiShard.shard._id_;

            rotation = uiShard.rotation;
            colorTime = uiShard.colorTime;

            FullRefresh();
        }

        public void PartialRefresh(float deltaTime) {
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy) return;

            // color
            colorTime += (deltaTime / colors.Length) * (Constants.UI.Shard.ColorAnimSpeed);
            if (colorTime > colors.Length) colorTime -= colors.Length;

            var colorMinIndex = (int)MathF.Floor(colorTime) % colors.Length;
            var colorMaxIndex = (int)MathF.Ceiling(colorTime) % colors.Length;

            var colorMin = shardConfig.GetColor(colors[colorMinIndex]);
            var colorMax = shardConfig.GetColor(colors[colorMaxIndex]);

            var color = colorMin != colorMax
                ? Color.Lerp(colorMin, colorMax, colorTime - colorMinIndex)
                : colorMin;

            shard.currentColor = color;
            level.SetColor(color);
            hover.SetColor(color);
            
            //

            
            // rotation
            var rotationSpeed = Constants.UI.Shard.RotationSpeed + (shard.level - 1) * Constants.UI.Shard.RotationSpeedLevelImpact;
            rotation += deltaTime * rotationSpeed;
            if (rotation > 360f) rotation -= 360f;

            level.SetRotation(rotation);
            body.transform.rotation = RotateUtils.GetByAngle(rotation * -1);
            //

            level.SetLevel(shard.level, shardConfig);
        }

        [Button]
        public void FullRefresh() {
            var quantity = ShardUtils.GetQuantity(ref shard);
            sectorsCount = 0;
            if (shard.red > 0) sectors[sectorsCount++] = new Sector { type = ShardTypes.Red, weight = (float)shard.red / quantity };
            if (shard.green > 0) sectors[sectorsCount++] = new Sector { type = ShardTypes.Green, weight = (float)shard.green / quantity };
            if (shard.blue > 0) sectors[sectorsCount++] = new Sector { type = ShardTypes.Blue, weight = (float)shard.blue / quantity };
            if (shard.yellow > 0) sectors[sectorsCount++] = new Sector { type = ShardTypes.Yellow, weight = (float)shard.yellow / quantity };
            if (shard.orange > 0) sectors[sectorsCount++] = new Sector { type = ShardTypes.Orange, weight = (float)shard.orange / quantity };
            if (shard.pink > 0) sectors[sectorsCount++] = new Sector { type = ShardTypes.Pink, weight = (float)shard.pink / quantity };
            if (shard.violet > 0) sectors[sectorsCount++] = new Sector { type = ShardTypes.Violet, weight = (float)shard.violet / quantity };
            if (shard.aquamarine > 0) sectors[sectorsCount++] = new Sector { type = ShardTypes.Aquamarine, weight = (float)shard.aquamarine / quantity };

            var isCanvas = IsCanvasMode;
            var layersCount = isCanvas ? layersIM.Length : layersSR.Length;

            var idx = 0;
            for (idx = 0; idx < layersCount; idx++) {
                if (isCanvas) layersIM[idx].gameObject.SetActive(idx < sectorsCount);
                else layersSR[idx].gameObject.SetActive(idx < sectorsCount);
            }

            if (sectorsCount == 0) return;

            var spritesCount = shardConfig.sectors.Length;

#if UNITY_EDITOR
            // colors
            if (colors.Length != Constants.UI.Shard.MaxCachedColors) {
                colors = new ShardTypes[Constants.UI.Shard.MaxCachedColors];
            }

            for (int colorIdx = 0; colorIdx < colors.Length; colorIdx++) {
                colors[colorIdx] = ShardTypes.Red;
            }
#endif

            var angle = 0f;
            var colorsFrom = 0;
            for (idx = 0; idx < sectorsCount; idx++) {
                var s = (int)(MathFast.Lerp(0, spritesCount - 2, sectors[idx].weight - 0.01f));

                var type = sectors[idx].type;
                var color = shardConfig.GetColor(type);
                var sprite = idx == 0 ? shardConfig.sectors[^1] : shardConfig.sectors[s];
                var r = Quaternion.AngleAxis(angle, Vector3.forward);

                if (isCanvas) {
                    layersIM[idx].color = color;
                    layersIM[idx].sprite = sprite;
                    layersIM[idx].transform.rotation = r;
                }
                else {
                    layersSR[idx].color = color;
                    layersSR[idx].sprite = sprite;
                    layersSR[idx].transform.rotation = r;
                }

                var segmentAngle = 360f * sectors[idx].weight;
                angle -= segmentAngle - 1f;

                // colors
                var colorsLen = (int)(colors.Length * sectors[idx].weight);
                for (var colorIdx = colorsFrom; colorIdx < colorsFrom + colorsLen && colorIdx < colors.Length; colorIdx++) {
                    colors[colorIdx] = type;
                }

                colorsFrom += colorsLen;
            }

            PartialRefresh(0f);
        }

        [Serializable]
        private struct Sector {
            public float weight;
            public ShardTypes type;
        }
    }
}
