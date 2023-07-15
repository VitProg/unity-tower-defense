using td.components.commands;
using td.components.flags;
using td.features.windows.common;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.windows
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

        private WindowsService windowsService => DI.Get<WindowsService>();

        void Awake()
        {
            startGameButton.onClick.AddListener(OnStartGameClicked); 
            choiseLevelButton.onClick.AddListener(OnChoiseLevelClicked); 
            exitButton.onClick.AddListener(OnExitClicked); 
            settingsButton.onClick.AddListener(OnSettingsClicked); 
            profileButton.onClick.AddListener(OnProfileClicked); 
        }

        private async void OnStartGameClicked()
        {
            // todo
            Debug.Log("OnStartGameClicked");

            DI.GetSystems().OuterSingle<LoadLevelOuterCommand>().levelNumber = 1;
            DI.GetSystems().OuterSingle<IsLoadingOuter>();
            DI.GetSystems().SetGroupSystemState(Constants.EcsSystemGroups.GameSimulation, false);
            await windowsService.CloseAll();
            DI.GetSystems().SetGroupSystemState(Constants.EcsSystemGroups.GameSimulation, true);
        }

        private async void OnChoiseLevelClicked()
        {
            // todo
            Debug.Log("OnChoiseLevelClicked");

            await windowsService.Open(WindowsService.Type.ChoiseLevelMenu);
        }

        private async void OnExitClicked()
        {
            // todo
            Debug.Log("OnExitClicked");

            var result = await windowsService.Open(WindowsService.Type.GameExitConfirm);

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

            await windowsService.Open(WindowsService.Type.SettingsMenu);
        }

        private async void OnProfileClicked()
        {
            // todo
            Debug.Log("OnProfileClicked");

            await windowsService.Open(WindowsService.Type.ProfilePopup);
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
