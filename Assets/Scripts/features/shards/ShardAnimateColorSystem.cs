using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.components.refs;
using td.features.shards.config;
using td.features.shards.mb;
using td.utils.ecs;
using UnityEngine;

namespace td.features.shards
{
    public class ShardAnimateColorSystem : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;
        [Inject] private ShardsConfig config;

        private readonly EcsFilterInject<Inc<Shard, Ref<GameObject>>, Exc<IsDisabled, IsDestroyed>> entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var shard = ref entities.Pools.Inc1.Get(entity);
                ref var go = ref entities.Pools.Inc2.Get(entity);

                if (world.HasComponent<ShardColor>(entity))
                {
                    ref var sc = ref world.GetComponent<ShardColor>(entity);
                    UpdateShardColor(ref sc, go.reference);
                }
                else
                {
                    InitShardColor(ref shard, entity, go.reference);
                }
            }
        }

        private void InitShardColor(ref Shard shard, int entity, GameObject go)
        {
            ref var sc = ref world.GetComponent<ShardColor>(entity);

            sc.colors = new List<ShardColor.Item>();
            
            var q = ShardUtils.GetQuantity(ref shard);
            
            if (shard.red > 0) sc.colors.Add(new ShardColor.Item { color = config.GetColorIndex("red"), weight = shard.red / (float)q });
            if (shard.green > 0) sc.colors.Add(new ShardColor.Item { color = config.GetColorIndex("green"), weight = shard.green / (float)q });
            if (shard.blue > 0) sc.colors.Add(new ShardColor.Item { color = config.GetColorIndex("blue"), weight = shard.blue / (float)q });
            if (shard.aquamarine > 0) sc.colors.Add(new ShardColor.Item { color = config.GetColorIndex("aquamarine"), weight = shard.aquamarine / (float)q });
            if (shard.yellow > 0) sc.colors.Add(new ShardColor.Item { color = config.GetColorIndex("yellow"), weight = shard.yellow / (float)q });
            if (shard.orange > 0) sc.colors.Add(new ShardColor.Item { color = config.GetColorIndex("orange"), weight = shard.orange / (float)q });
            if (shard.pink > 0) sc.colors.Add(new ShardColor.Item { color = config.GetColorIndex("pink"), weight = shard.pink / (float)q });
            if (shard.violet > 0) sc.colors.Add(new ShardColor.Item { color = config.GetColorIndex("violet"), weight = shard.violet / (float)q });

            sc.animate = sc.colors.Count > 1;
            sc.currentColor = 0;
            sc.nextColor = sc.colors.Count > 1 ? 1 : 0;
            sc.prevColor = sc.colors.Count - 1;
            sc.resultColor = config.GetColor(sc.colors[sc.currentColor].color);
            if (sc.colors.Count > 0)
            {
                SetColor(config.GetColor(sc.colors[sc.currentColor].color), go);
            }
            else
            {
                sc.colors.Clear();
            }
        }


        private void UpdateShardColor(ref ShardColor sc, GameObject go)
        {
            if (!sc.animate) return;

            sc.colorTime += Time.deltaTime / 5f;

            var prev = sc.colors[sc.prevColor];
            var current = sc.colors[sc.currentColor];
            var next = sc.colors[sc.nextColor];

            var color = config.GetColor(current.color);

            var w = sc.colors[sc.currentColor].weight * 5f;
            const float fade = .1f;

            if (sc.colorTime < fade)
            {
                var t = sc.colorTime / fade;
                var from = config.GetColor(prev.color);
                var to = config.GetColor(current.color);
                color = Color.Lerp(from, to, t);
            }
            else if (sc.colorTime > w + fade)
            {
                var t = ((current.weight + fade + fade) - sc.colorTime) / fade;
                var from = config.GetColor(current.color);
                var to = config.GetColor(next.color);
                color = Color.Lerp(from, to, t);
            }

            SetColor(color, go);
            sc.resultColor = color;
            
            if (sc.colorTime > sc.colors[sc.currentColor].weight + fade + fade)
            {
                sc.colorTime = 0f;
                sc.currentColor = (sc.currentColor + 1) % sc.colors.Count;
                sc.nextColor = (sc.currentColor + 1) % sc.colors.Count;
                sc.prevColor = (sc.currentColor - 1 + sc.colors.Count) % sc.colors.Count;
            }
        }

        private void SetColor(Color color, GameObject go)
        {
            go.GetComponent<ShardMonoBehaviour>().SetLevelAndHoverColor(color);
        }
    }
}