using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace td.features.shard.mb
{
    public class ShardLevelIndicatorMB : MonoBehaviour
    {
        public Image image;
        public SpriteRenderer spriteRenderer;

        public float colorTime = 0f;
        public float rotation = 0f;
        public int level;

        public void SetLevel(int l, ShardsConfig config)
        {
            level = l;
            if (config.levelSprites.Length > level - 1)
            {
                if (image) image.sprite = config.levelSprites[level - 1];
                if (spriteRenderer) spriteRenderer.sprite = config.levelSprites[level - 1];
            }
        }

        public void SetColor(Color color)
        {
            if (spriteRenderer) spriteRenderer.color = color;
            if (image) image.color = color;
        }

        public void SetRotation(float r)
        {
            rotation = r;
            transform.rotation = Quaternion.AngleAxis(-rotation, Vector3.forward);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}