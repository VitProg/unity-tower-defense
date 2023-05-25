using System;
using System.Collections;
using NaughtyAttributes;
using td.features.shards.config;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.shards.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    [ExecuteAlways]
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

        [ShowNativeProperty]
        public uint Quantity => (uint)red + green + blue + aquamarine + yellow + orange + pink + violet;

        public byte[] Values => new[] { red, green, blue, aquamarine, yellow, orange, pink, violet };

        private uint lastQuantity = 0;

        [SerializeField][Required] public EcsEntity ecsEntity;
        [FormerlySerializedAs("circleMeshGenerator")] [SerializeField][Required] private ShardMeshGenerator shardMeshGenerator;
        private ShardCalculator shardCalculator;

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
            
            var i = 0;

            shardMeshGenerator.colorWeights[i++] = red;
            shardMeshGenerator.colorWeights[i++] = green;
            shardMeshGenerator.colorWeights[i++] = blue;
            shardMeshGenerator.colorWeights[i++] = aquamarine;
            shardMeshGenerator.colorWeights[i++] = yellow;
            shardMeshGenerator.colorWeights[i++] = orange;
            shardMeshGenerator.colorWeights[i++] = pink;
            shardMeshGenerator.colorWeights[i++] = violet;

            if (HasShard())
            {
                ref var shard = ref GetShard();

                var level = shardCalculator!.GetShardLevel(ref shard);

                shardMeshGenerator.innerRadius = Mathf.Lerp(
                    0.6f,
                    0.15f,
                    level / 15f
                );

                // shardMeshGenerator.innerRadius = Mathf.Lerp(
                // 0.6f,
                // 0.2f,
                // Mathf.Max(red, green, blue, aquamarine, yellow, orange, pink, violet) / 100f
                // );
            }

            shardMeshGenerator.Refresh();
        }
        
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
    }
}