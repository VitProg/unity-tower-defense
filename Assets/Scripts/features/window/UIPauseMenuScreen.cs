using Leopotam.EcsLite;
using td.features.window.common;
using td.utils.di;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.window
{
    [RequireComponent(typeof(FadeInOut))]
    [RequireComponent(typeof(UIWindowPopup))]
    public class UIPauseMenuScreen : MonoInjectable
    {
        private readonly EcsInject<Windows_Service> windowsService;

        [SerializeField] private Button closeButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private new void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(OnCloseClicked);
            resumeButton.onClick.AddListener(OnCloseClicked);
            restartButton.onClick.AddListener(OnRestartClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnQuitClicked()
        {
            Debug.Log("OnQuitClicked");
            //todo
        }

        private async void OnSettingsClicked()
        {
            Debug.Log("OnSettingsClicked");
            await windowsService.Value.Open(Windows_Service.Type.SettingsMenu);
        }

        private void OnRestartClicked()
        {
            Debug.Log("OnRestartClicked");
            //todo
        }

        private async void OnCloseClicked()
        {
            Debug.Log("OnCloseClicked");
            await windowsService.Value.Close(Windows_Service.Type.PauseMenu);
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
