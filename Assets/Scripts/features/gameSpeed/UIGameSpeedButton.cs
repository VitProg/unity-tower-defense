using NaughtyAttributes;
using UnityEngine;
using td.utils.ecs;
using td.features.state;
using td.utils;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace td.features.gameSpeed
{
    public class UIGameSpeedButton : MonoBehaviour, IPointerDownHandler
    {
        [Inject] private State state;
        
        [Required][SerializeField] private Image image;
        
        [Required][SerializeField] private float gameSpeed;
        
        [Required][SerializeField] private Sprite onStateSprite;
        [Required][SerializeField] private Sprite offStateSprite;
        
        public void Refresh()
        {
            if (state == null)
            {
                DI.Resolve(this);
            }
            
            var isOn = FloatUtils.IsEquals(state!.GameSpeed, gameSpeed);
            image.sprite = isOn ? onStateSprite : offStateSprite;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (state == null)
            {
                DI.Resolve(this);
            }
            
            state!.GameSpeed = gameSpeed;
        }
    }
}