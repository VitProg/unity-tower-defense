using UnityEngine;
using UnityEngine.UI;

namespace td.features.shards.mb
{
    public class ShardCollectionPanel : MonoBehaviour
    {
        public GridLayoutGroup grid;

        private void Start()
        {
            grid ??= GetComponent<GridLayoutGroup>();
        }
    }
}