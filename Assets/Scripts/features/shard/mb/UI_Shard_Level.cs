using td.features.shard.data;
using td.utils;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shard.mb
{
    public class UI_Shard_Level : MonoBehaviour
    {
        public Image image;
        public SpriteRenderer spriteRenderer;
        
        public float rotation = 0f;
        public uint level;

        public void SetLevel(uint l, in Shards_Config_SO configSO)
        {
            if (level == l) return;
            level = l;
            if (level < configSO.levelSprites.Length)
            {
                if (image) image.sprite = configSO.levelSprites[level];
                if (spriteRenderer) spriteRenderer.sprite = configSO.levelSprites[level];
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
            transform.rotation = RotateUtils.GetByAngle(r);//Quaternion.AngleAxis(-rotation, Vector3.forward));
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetupFrom(UI_Shard_Level source)
        {
            level = source.level;
            rotation = source.rotation;
        }
    }
}