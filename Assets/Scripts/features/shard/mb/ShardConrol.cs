using NaughtyAttributes;
using td.features.shard.components;
using td.utils.di;
using UnityEngine;
using UnityEngine.EventSystems;

namespace td.features.shard.mb
{
    public class ShardConrol : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [Required] public ShardMonoBehaviour shardMB;
        
        public bool IsHovered => shardMB.IsHovered;

        [Button]
        public void Refresh()
        {
            shardMB.Refresh();
        }

        public void SetShard(ref Shard shard)
        {
            shardMB.SetShard(ref shard);
        }
        
        public ref Shard GetShard()
        {
            return ref shardMB.shardData;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log("OnPointerEnter");
            
            shardMB.hover.Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Debug.Log("OnPointerExit");
            
            shardMB.hover.Hide();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Debug.Log("Click");
            
            // throw new System.NotImplementedException();
        }
    }
}