using UnityEngine;
using UnityEngine.UI;

namespace td.features.shard.mb
{
    public class ShardHoverMB : MonoBehaviour
    {
        public Image image;
        public SpriteRenderer spriteRenderer;
        public Animation anim;

        // private bool inHideProcess = false;
        // private bool inShowProcess = false;
        
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

        public void Show()
        {
            gameObject.SetActive(true);
            anim.Play();
        }
        
        public void Hide()
        {
            anim.Stop();
            gameObject.SetActive(false);
        }
        
        /*
        public async Task Show()
        {
            inHideProcess = false;
            inShowProcess = true;
            
            anim.Stop();
            
            var c = GetColor();
            gameObject.SetActive(true);
            for (var a = 0f; a < 1f && inShowProcess; a += Time.deltaTime)
            {
                c.a = a;
                SetColor(c);
                await Task.Delay(100);
                await Task.Yield();
            }

            c.a = 1f;
            SetColor(c);
            
            inShowProcess = false;
            anim.Play();
        }

        public async Task Hide()
        {
            inHideProcess = true;
            inShowProcess = false;
            
            anim.Stop();

            var c = GetColor();
            gameObject.SetActive(true);
            for (var a = 1f; a > 0f && inHideProcess; a -= Time.deltaTime)
            {
                c.a = a;
                SetColor(c);
                await Task.Delay(100);
                await Task.Yield();
            }

            c.a = 0f;
            SetColor(c);
            gameObject.SetActive(false);
            
            inHideProcess = false;
        }
*/
        public bool IsVisible => gameObject.activeSelf;
    }
}