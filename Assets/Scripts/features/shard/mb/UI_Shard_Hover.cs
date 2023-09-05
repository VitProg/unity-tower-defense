using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shard.mb
{
    public class UI_Shard_Hover : MonoBehaviour
    {
        public Image image;
        public SpriteRenderer spriteRenderer;
        public Animation anim;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetColor(Color color)
        {
            if (spriteRenderer) spriteRenderer.color = color;
            if (image) image.color = color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Color GetColor()
        {
            if (spriteRenderer) return spriteRenderer.color;
            if (image) return image.color;
            return Color.black;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Show()
        {
            gameObject.SetActive(true);
            anim.Play();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Hide()
        {
            anim.Stop();
            gameObject.SetActive(false);
        }

        public bool IsVisible
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => gameObject.activeSelf;
        }
    }
}