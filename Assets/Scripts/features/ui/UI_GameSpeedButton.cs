using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using td.features.eventBus;
using td.features.state;
using td.features.state.bus;
using td.features.window;
using td.utils;
using td.utils.di;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace td.features.ui
{
    public class UI_GameSpeedButton : MonoBehaviour, IPointerDownHandler
    {
        private State _state;
        private State State => _state ??= ServiceContainer.Get<State>();
        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();
        private Window_Service _windowService;
        private Window_Service WindowService => _windowService ??= ServiceContainer.Get<Window_Service>();
        
        [Required][SerializeField] private Image image;
        
        [SerializeField] private float gameSpeed = 1.0f;
        
        [Required][SerializeField] private Sprite onStateSprite;
        [Required][SerializeField] private Sprite offStateSprite;
        
        public void Start()
        {
            Events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnStateChanged(ref Event_StateChanged ev)
        {
            if (ev.gameSpeed)
            {
                Refresh();
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void Refresh()
        {
            var isOn = FloatUtils.IsEquals(State.GetGameSpeed(), gameSpeed);
            image.sprite = isOn ? onStateSprite : offStateSprite;
        }

        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDownAsync(eventData).Forget();
        }

        private async UniTaskVoid OnPointerDownAsync(PointerEventData eventData)
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