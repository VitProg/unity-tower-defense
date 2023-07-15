using td.features.windows.common;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.windows
{
    [RequireComponent(typeof(FadeInOut))]
    [RequireComponent(typeof(UIWindowPopup))]
    public class UIPauseMenuScreen : MonoBehaviour
    {
        private WindowsService windowsService => DI.Get<WindowsService>();

        [SerializeField] private Button closeButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        void Awake()
        {
            closeButton.onClick.AddListener(OnCloseClicked);
            resumeButton.onClick.AddListener(OnCloseClicked);
            restartButton.onClick.AddListener(OnRestartClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        private async void OnQuitClicked()
        {
            Debug.Log("OnQuitClicked");
            //todo
        }

        private async void OnSettingsClicked()
        {
            Debug.Log("OnSettingsClicked");
            await windowsService.Open(WindowsService.Type.SettingsMenu);
        }

        private async void OnRestartClicked()
        {
            Debug.Log("OnRestartClicked");
            //todo
        }

        private async void OnCloseClicked()
        {
            Debug.Log("OnCloseClicked");
            await windowsService.Close(WindowsService.Type.PauseMenu);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.RemoveAllListeners();
            restartButton.onClick.RemoveAllListeners();
            settingsButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
        }
    }
}
