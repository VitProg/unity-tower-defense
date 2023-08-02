using System.Threading.Tasks;
using Leopotam.EcsLite;
using td.features._common;
using td.features._common.bus;
using td.features.gameStatus.bus;
using td.features.level.bus;
using td.features.window.common;
using td.utils.di;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.window
{
    [RequireComponent(typeof(FadeInOut))]
    [RequireComponent(typeof(UIWindowScreen))]
    public class UIMainMenuScreen : MonoInjectable
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button choiseLevelButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button profileButton;

        private readonly EcsInject<Windows_Service> windowsService;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Common_Service> common;

        private new void Awake()
        {
            base.Awake();
            startGameButton.onClick.AddListener(OnStartGameClicked); 
            choiseLevelButton.onClick.AddListener(OnChoiseLevelClicked); 
            exitButton.onClick.AddListener(OnExitClicked); 
            settingsButton.onClick.AddListener(OnSettingsClicked); 
            profileButton.onClick.AddListener(OnProfileClicked);
#if TD_AUTO_START_LEVEL
            OnStartGameClicked();
#endif
        }

        private async void OnStartGameClicked()
        {
            Debug.Log("OnStartGameClicked");

            events.Value.Unique.Add<Command_StopGameSimulation>();
            events.Value.Unique.Add<Command_LoadLevel>().levelNumber = 1;
            
            await Task.Yield();
            await Task.Delay(500);

            await Task.Yield();
            await windowsService.Value.CloseAll();
            
            await Task.Yield();
            await Task.Delay(300);
            
            events.Value.Unique.Add<Command_StartGame>();
        }

        private async void OnChoiseLevelClicked()
        {
            // todo
            Debug.Log("OnChoiseLevelClicked");

            await windowsService.Value.Open(Windows_Service.Type.ChoiseLevelMenu);
        }

        private async void OnExitClicked()
        {
            // todo
            Debug.Log("OnExitClicked");

            var result = await windowsService.Value.Open(Windows_Service.Type.GameExitConfirm);

            if (result)
            {
                // todo exit game
                Application.Quit();
            }
        }

        private async void OnSettingsClicked()
        {
            // todo
            Debug.Log("OnSettingsClicked");

            await windowsService.Value.Open(Windows_Service.Type.SettingsMenu);
        }

        private async void OnProfileClicked()
        {
            // todo
            Debug.Log("OnProfileClicked");

            await windowsService.Value.Open(Windows_Service.Type.ProfilePopup);
        }

        private void OnDestroy()
        {
            startGameButton.onClick.RemoveAllListeners(); 
            choiseLevelButton.onClick.RemoveAllListeners(); 
            exitButton.onClick.RemoveAllListeners(); 
            settingsButton.onClick.RemoveAllListeners(); 
            profileButton.onClick.RemoveAllListeners(); 
        }
    }
}
