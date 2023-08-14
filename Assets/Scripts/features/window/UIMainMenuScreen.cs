using System.Threading.Tasks;
using JetBrains.Annotations;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.eventBus;
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
    public class UIMainMenuScreen : MonoBehaviour
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button choiseLevelButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button profileButton;

        private Window_Service WindowsService =>  ServiceContainer.Get<Window_Service>();
        private EventBus Events => ServiceContainer.Get<EventBus>();

        private void Start()
        {
            startGameButton.onClick.AddListener(OnStartGameClicked); 
            choiseLevelButton.onClick.AddListener(OnChoiseLevelClicked); 
            exitButton.onClick.AddListener(OnExitClicked); 
            settingsButton.onClick.AddListener(OnSettingsClicked); 
            profileButton.onClick.AddListener(OnProfileClicked);
// #if TD_AUTO_START_LEVEL
//             OnStartGameClicked();
// #endif
        }

        private async void OnStartGameClicked()
        {
            Debug.Log("OnStartGameClicked");

            Events.unique.GetOrAdd<Command_StopGameSimulation>();
            Events.unique.GetOrAdd<Command_LoadLevel>();//.levelNumber = 1;
            
            await Task.Yield();
            await Task.Delay(500);

            await Task.Yield();
            await WindowsService.CloseAll();
            
            await Task.Yield();
            await Task.Delay(300);
            
            Events.unique.GetOrAdd<Command_StartGame>();
        }

        private async void OnChoiseLevelClicked()
        {
            // todo
            Debug.Log("OnChoiseLevelClicked");

            await WindowsService.Open(Window_Service.Type.ChoiseLevelMenu);
        }

        private async void OnExitClicked()
        {
            // todo
            Debug.Log("OnExitClicked");

            var result = await WindowsService.Open(Window_Service.Type.GameExitConfirm);

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

            await WindowsService.Open(Window_Service.Type.SettingsMenu);
        }

        private async void OnProfileClicked()
        {
            // todo
            Debug.Log("OnProfileClicked");

            await WindowsService.Open(Window_Service.Type.ProfilePopup);
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
