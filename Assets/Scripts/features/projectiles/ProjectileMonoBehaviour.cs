using UnityEngine;

namespace td.features.projectiles
{
    public class ProjectileMonoBehaviour : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public TrailRenderer trailRenderer;

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
            trailRenderer.startColor = color;
            trailRenderer.endColor = color;
        } 
    }
}