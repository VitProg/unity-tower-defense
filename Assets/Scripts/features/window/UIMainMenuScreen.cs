using Cysharp.Threading.Tasks;
using td.features.eventBus;
using td.features.level.bus;
using td.features.state;
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

        private Window_Service _windowsService;
        private Window_Service WindowsService => _windowsService ??= ServiceContainer.Get<Window_Service>();
        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();
        private State _state;
        private State State => _state ??= ServiceContainer.Get<State>();

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

        private void OnStartGameClicked()
        {
            StartGame().Forget();
        }
        
        private async UniTaskVoid StartGame()
        {
            Debug.Log("OnStartGameClicked");

            //todo
            State.SetSimulationEnabled(false);
            Events.unique.GetOrAdd<Command_LoadLevel>();
            
            await UniTask.Yield();
            await UniTask.Delay(500);

            await UniTask.Yield();
            await WindowsService.CloseAll();
            
            await UniTask.Yield();
            await UniTask.Delay(300);
            
            State.SetSimulationEnabled(true);
            
            // Events.unique.GetOrAdd<Command_StartGame>();
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
