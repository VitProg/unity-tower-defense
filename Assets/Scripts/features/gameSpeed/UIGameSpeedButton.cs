using System;
using Leopotam.EcsLite;
using NaughtyAttributes;
using UnityEngine;
using td.features.state;
using td.features.window;
using td.utils;
using td.utils.di;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace td.features.gameSpeed
{
    public class UIGameSpeedButton : MonoInjectable, IPointerDownHandler
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Windows_Service> windowsService;
        
        [Required][SerializeField] private Image image;
        
        [SerializeField] private float gameSpeed = 1.0f;
        
        [Required][SerializeField] private Sprite onStateSprite;
        [Required][SerializeField] private Sprite offStateSprite;
        
        private IDisposable eventDispose;

        // private bool diResolved;

        // public new void Awake()
        // {
            // base.Awake();
            // eventBus.Value.Subscribe(this);
            // await DI.Resolve(this);
            // DI.Get<EventBus>()!.Subscribe(this);
            // diResolved = true;
        // }

        public void Start()
        {
            eventDispose = events.Value.Unique.SubscribeTo<Event_StateChanged>(OnStateChanged);
        }

        private void OnStateChanged(ref Event_StateChanged @event)
        {
            if (@event.gameSpeed.HasValue)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            var isOn = FloatUtils.IsEquals(state.Value.GameSpeed, gameSpeed);
            image.sprite = isOn ? onStateSprite : offStateSprite;
        }

        private void OnDestroy()
        {
            eventDispose.Dispose();
        }

        public async void OnPointerDown(PointerEventData eventData)
        {
            // if (!diResolved) return;
            var lastGameSpeed = state.Value.GameSpeed;
            
            state.Value.GameSpeed = gameSpeed;

            if (FloatUtils.IsZero(gameSpeed))
            {
                await windowsService.Value.Open(Windows_Service.Type.PauseMenu);
                await windowsService.Value.WaitClose(Windows_Service.Type.PauseMenu);
                state.Value.GameSpeed = lastGameSpeed;
            }
        }
    }
}