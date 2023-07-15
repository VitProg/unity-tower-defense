using NaughtyAttributes;
using td.features.eventBus;
using UnityEngine;
using td.utils.ecs;
using td.features.state;
using td.features.windows;
using td.utils;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace td.features.gameSpeed
{
    public class UIGameSpeedButton : MonoBehaviour, IPointerDownHandler, IEventReceiver<StateChangedEvent>
    {
        public UniqueId Id { get; } = new UniqueId();
        
        [Inject] private State state;
        [Inject] private EventBus eventBus;
        
        [Required][SerializeField] private Image image;
        
        [SerializeField] private float gameSpeed = 1.0f;
        
        [Required][SerializeField] private Sprite onStateSprite;
        [Required][SerializeField] private Sprite offStateSprite;
        
        private bool diResolved;

        public async void Awake()
        {
            await DI.Resolve(this);
            DI.Get<EventBus>()!.Subscribe(this);
            diResolved = true;
        }

        public void Refresh()
        {
            var isOn = FloatUtils.IsEquals(state!.GameSpeed, gameSpeed);
            image.sprite = isOn ? onStateSprite : offStateSprite;
        }
        
        public async void OnPointerDown(PointerEventData eventData)
        {
            if (!diResolved) return;
            var lastGameSpeed = state!.GameSpeed;
            
            state!.GameSpeed = gameSpeed;

            if (FloatUtils.IsZero(gameSpeed))
            {
                var windowsService = DI.Get<WindowsService>()!;
                await windowsService.Open(WindowsService.Type.PauseMenu);
                await windowsService.WaitClose(WindowsService.Type.PauseMenu);
                state.GameSpeed = lastGameSpeed;
            }
        }

        public void OnEvent(StateChangedEvent @event)
        {
            Refresh();
        }
    }
}