using NaughtyAttributes;
using td.features.eventBus;
using td.features.state;
using td.features.window;
using td.utils;
using td.utils.di;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace td.features.gameSpeed
{
    public class UIGameSpeedButton : MonoBehaviour, IPointerDownHandler
    {
        private State State => ServiceContainer.Get<State>();
        private EventBus Events => ServiceContainer.Get<EventBus>();
        private Window_Service WindowService => ServiceContainer.Get<Window_Service>();
        
        [Required][SerializeField] private Image image;
        
        [SerializeField] private float gameSpeed = 1.0f;
        
        [Required][SerializeField] private Sprite onStateSprite;
        [Required][SerializeField] private Sprite offStateSprite;
        
        public void Start()
        {
            Events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
        }

        private void OnStateChanged(ref Event_StateChanged ev)
        {
            if (ev.gameSpeed)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            var isOn = FloatUtils.IsEquals(State.GetGameSpeed(), gameSpeed);
            image.sprite = isOn ? onStateSprite : offStateSprite;
        }

        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
        }

        public async void OnPointerDown(PointerEventData eventData)
        {
            // if (!diResolved) return;
            var lastGameSpeed = State.GetGameSpeed();
            
            State.SetGameSpeed(gameSpeed);
            // Time.timeScale = gameSpeed;

            if (FloatUtils.IsZero(gameSpeed))
            {
                await WindowService.Open(Window_Service.Type.PauseMenu);
                await WindowService.WaitClose(Window_Service.Type.PauseMenu);
                State.SetGameSpeed(lastGameSpeed);
                // Time.timeScale = lastGameSpeed;
            }
        }
    }
}