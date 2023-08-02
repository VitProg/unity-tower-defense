using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shard.mb
{
    public class ShardDenyMB : MonoBehaviour
    {
        public Image image;
        public SpriteRenderer spriteRenderer;
        
        public void SetColor(Color color)
        {
            if (spriteRenderer) spriteRenderer.color = color;
            if (image) image.color = color;
        }

        private Color GetColor()
        {
            if (spriteRenderer) return spriteRenderer.color;
            if (image) return image.color;
            return Color.black;
        }
        
        public void SetVisible(bool visible)
        {
            if (visible) Show();
            else Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool IsVisible => gameObject.activeSelf;
    }
}